using StreamProcessing.SqlExecutor.Domain;

namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface IDmlExecutor
{
    //For sake of performance we dont create command or connection every time
    Task Execute(IStreamDbConnection connection,
        IStreamDbCommand command,
        DmlCommand dmlCommand, 
        IReadOnlyDictionary<string, object>? record, 
        CancellationToken cancellationToken);
}