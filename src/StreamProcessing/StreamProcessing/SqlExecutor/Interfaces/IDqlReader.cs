using StreamProcessing.SqlExecutor.Domain;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IDqlReader
{
    //For sake of performance we dont create command or connection every time
    IAsyncEnumerable<IReadOnlyDictionary<string, object>>  
        Read(IStreamDbConnection connection,
            IStreamDbCommand command,
            DqlCommand dqlCommand, 
            IReadOnlyDictionary<string, object>? record, 
            CancellationToken cancellationToken);
}