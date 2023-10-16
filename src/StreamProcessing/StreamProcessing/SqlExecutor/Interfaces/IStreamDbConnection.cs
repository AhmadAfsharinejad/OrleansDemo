using System.Data;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IStreamDbConnection : IDbConnection, IAsyncDisposable
{
    Task OpenAsync();
    Task OpenAsync(CancellationToken cancellationToken);
    IDbConnection BaseConnection { get; }
    IStreamDbCommand CreateStreamDbCommand();
}