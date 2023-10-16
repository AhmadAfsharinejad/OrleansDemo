namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IConnectionFactory
{
    IStreamDbConnection Create(string connectionString);
}