namespace StreamProcessing.SqlExecutor.Interfaces;

internal interface ICommandFiller
{
    //For sake of performance we dont create command or connection every time 
    void Fill(IStreamDbConnection connection, 
        IStreamDbCommand command,
        string commandText,
        IReadOnlyCollection<string>? parameterFields,
        IReadOnlyDictionary<string, object>? record);
}