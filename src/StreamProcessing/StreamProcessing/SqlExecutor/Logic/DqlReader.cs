using System.Data;
using System.Runtime.CompilerServices;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

internal sealed class DqlReader : IDqlReader
{
    private readonly ICommandFiller _commandFiller;

    public DqlReader(ICommandFiller commandFiller)
    {
        _commandFiller = commandFiller ?? throw new ArgumentNullException(nameof(commandFiller));
    }

    public async IAsyncEnumerable<IReadOnlyDictionary<string, object>> 
        Read(IStreamDbConnection connection,
            IStreamDbCommand command,
            DqlCommand dqlCommand, 
            IReadOnlyDictionary<string, object>? record, 
            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _commandFiller.Fill(connection, command, dqlCommand.CommandText, dqlCommand.ParameterFileds, record);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return Read(reader, dqlCommand.OutputFileds);
        }

        await Task.CompletedTask;
    }

    private static IReadOnlyDictionary<string, object> Read(IDataRecord reader, IEnumerable<DqlField> outputFileds)
    {
        var record = new Dictionary<string, object>();

        foreach (var field in outputFileds)
        {
            record[field.Field.Name] = reader[field.DbName];
        }

        return record;
    }
}