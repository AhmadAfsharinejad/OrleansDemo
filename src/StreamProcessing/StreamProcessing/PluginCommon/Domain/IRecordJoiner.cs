namespace StreamProcessing.PluginCommon.Domain;

internal interface IRecordJoiner
{
    PluginRecord Join(PluginRecord? input, IReadOnlyDictionary<string, object>? computeResult, RecordJoinType joinType);
}