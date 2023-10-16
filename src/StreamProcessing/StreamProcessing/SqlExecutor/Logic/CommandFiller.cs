using System.Diagnostics.CodeAnalysis;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")] //TODO why not support?
internal sealed class CommandFiller : ICommandFiller
{
    public void Fill(IStreamDbConnection connection,
        IStreamDbCommand command,
        string commandText, 
        IReadOnlyCollection<string>? parameterFields, 
        IReadOnlyDictionary<string, object>? record)
    {
        command.CommandText = commandText;

        if (parameterFields is null) return;

        foreach (var parameter in parameterFields)
        {
            command.AddParameterWithValue(record![parameter]);
        }
    }
}