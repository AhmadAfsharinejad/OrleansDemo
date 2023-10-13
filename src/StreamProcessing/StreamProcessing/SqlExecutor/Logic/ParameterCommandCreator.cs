using System.Data;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")] //TODO why not support?
internal sealed class ParameterCommandCreator : IParameterCommandCreator
{
    public IDbCommand Create(OdbcConnection connection, string commandText, 
        IReadOnlyCollection<string>? ParameterFileds, 
        IReadOnlyDictionary<string, object>? record)
    {
        var command = connection.CreateCommand();
        command.CommandText = commandText;

        if (ParameterFileds is null) return command;

        foreach (var parameter in ParameterFileds)
        {
            command.Parameters.AddWithValue(null, record![parameter]);
        }

        return command;
    }
}