using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed class ConnectionFactory : IConnectionFactory
{
    public IStreamDbConnection Create(string connectionString)
    {
        return new StreamDbConnection(new OdbcConnection(connectionString));
    }
}