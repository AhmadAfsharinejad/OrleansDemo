using System.Data.Odbc;
using StreamProcessing.SqlExecutor.Domain;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IDmlExecutor
{
    Task Execute(OdbcConnection connection, DmlCommand dmlCommand, IReadOnlyDictionary<string, object>? record);
}