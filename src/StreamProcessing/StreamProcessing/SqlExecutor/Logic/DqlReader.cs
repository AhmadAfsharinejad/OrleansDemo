using System.Data;
using System.Data.Odbc;
using System.Runtime.CompilerServices;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

internal sealed class DqlReader : IDqlReader
{
    private readonly IParameterCommandCreator _commandCreator;

    public DqlReader(IParameterCommandCreator commandCreator)
    {
        _commandCreator = commandCreator ?? throw new ArgumentNullException(nameof(commandCreator));
    }

    public async IAsyncEnumerable<IReadOnlyDictionary<string, object>> 
        Read(OdbcConnection connection, DqlCommand dqlCommand, 
            IReadOnlyDictionary<string, object>? record, 
            [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var command = _commandCreator.Create(connection, dqlCommand.CommandText, dqlCommand.ParameterFileds, record);
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