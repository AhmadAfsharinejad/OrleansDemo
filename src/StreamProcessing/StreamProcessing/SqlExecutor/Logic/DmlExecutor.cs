using System.Data.Odbc;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

internal sealed class DmlExecutor : IDmlExecutor
{
    private readonly IParameterCommandCreator _commandCreator;

    public DmlExecutor(IParameterCommandCreator commandCreator)
    {
        _commandCreator = commandCreator ?? throw new ArgumentNullException(nameof(commandCreator));
    }
    
    public async Task Execute(OdbcConnection connection, 
        DmlCommand dmlCommand, 
        IReadOnlyDictionary<string, object>? record)
    {
        using var command = _commandCreator.Create(connection, dmlCommand.CommandText, dmlCommand.ParameterFileds, record);
        command.ExecuteNonQuery();
        //Note:ExecuteNonQueryAsync call ExecuteNonQuery and return Task.FromResult!
        await Task.CompletedTask;
    }
}