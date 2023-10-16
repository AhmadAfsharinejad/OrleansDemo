using System.Data;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IStreamDbCommand : IDbCommand
{
    void AddParameterWithValue(object? value);
}