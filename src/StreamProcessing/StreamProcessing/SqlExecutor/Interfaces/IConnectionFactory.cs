using System.Data.Odbc;

namespace StreamProcessing.SqlExecutor.Interfaces;

//TODO 
internal interface IConnectionFactory
{
    OdbcConnection Create(string connectionString);
}