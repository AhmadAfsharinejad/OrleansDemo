using System.Data.Odbc;
using System.Runtime.CompilerServices;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

internal sealed class SqlExecutorService : ISqlExecutorService
{
    private readonly IDmlExecutor _dmlExecutor;
    private readonly IDqlReader _dqlReader;
    private readonly IRecordJoiner _recordJoiner;

    public SqlExecutorService(IDmlExecutor dmlExecutor, IDqlReader dqlReader, IRecordJoiner recordJoiner)
    {
        _dmlExecutor = dmlExecutor ?? throw new ArgumentNullException(nameof(dmlExecutor));
        _dqlReader = dqlReader ?? throw new ArgumentNullException(nameof(dqlReader));
        _recordJoiner = recordJoiner ?? throw new ArgumentNullException(nameof(recordJoiner));
    }

    public async IAsyncEnumerable<PluginRecord> Execute(OdbcConnection connection, 
        SqlExecutorConfig config,
        PluginRecord? record,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (config.DmlCommands is not null)
        {
            await RunDmlCommands(connection, config.DmlCommands, record?.Record, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (config.DqlCommand is null)
        {
            yield return _recordJoiner.Join(record, null, config.JoinType);
            yield break;
        }

        await foreach (var readRecord in ReadDqlCommand(connection, config.DqlCommand.Value, record, config.JoinType, cancellationToken))
        {
            yield return readRecord;
        }
    }

    private async Task RunDmlCommands(OdbcConnection connection, 
        IEnumerable<DmlCommand> dmlCommands,
        IReadOnlyDictionary<string, object>? record,
        CancellationToken cancellationToken)
    {
        foreach (var dmlCommand in dmlCommands)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _dmlExecutor.Execute(connection, dmlCommand, record);
        }
    }

    private async IAsyncEnumerable<PluginRecord> ReadDqlCommand(OdbcConnection connection, 
        DqlCommand dqlCommand,
        PluginRecord? record, 
        RecordJoinType joinType,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var readResult in _dqlReader.Read(connection, dqlCommand, record?.Record, cancellationToken))
        {
            yield return _recordJoiner.Join(record, readResult, joinType);
        }
    }
}