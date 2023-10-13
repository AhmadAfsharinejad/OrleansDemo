using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Logic;

internal sealed class RecordJoiner : IRecordJoiner
{
    public PluginRecord Join(PluginRecord? input, IReadOnlyDictionary<string, object>? computeResult, RecordJoinType joinType)
    {
        return joinType switch
        {
            RecordJoinType.InputOnly => input ?? throw new InvalidOperationException("Input can't be null."),
            RecordJoinType.ResultOnly => Convert(computeResult),
            RecordJoinType.Append => Append(input, computeResult),
            _ => throw new ArgumentOutOfRangeException(nameof(joinType), joinType, null)
        };
    }

    private static PluginRecord Convert(IReadOnlyDictionary<string, object>? computeResult)
    {
        if (computeResult is null)
        {
            throw new InvalidOperationException("Compute can't be null.");
        }

        return new PluginRecord(computeResult);
    }

    private static PluginRecord Append(PluginRecord? input, IReadOnlyDictionary<string, object>? computeResult)
    {
        if (computeResult is null && input is null)
        {
            throw new InvalidOperationException("Both input and compute can't be null.");
        }

        var result = input is null ? new Dictionary<string, object>() : new Dictionary<string, object>(input.Value.Record);

        if (computeResult is null)
        {
            return new PluginRecord(result);
        }

        foreach (var keyValue in computeResult)
        {
            result.Add(keyValue.Key, keyValue.Value);
        }

        return new PluginRecord(result);
    }
}