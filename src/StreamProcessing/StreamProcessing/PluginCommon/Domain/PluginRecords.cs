namespace StreamProcessing.PluginCommon.Domain;

[Immutable]
internal record struct PluginRecords
{
    public IReadOnlyCollection<PluginRecord> Records;
}