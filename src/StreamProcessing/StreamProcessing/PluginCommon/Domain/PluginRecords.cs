namespace StreamProcessing.PluginCommon.Domain;

internal record struct PluginRecords
{
    public IReadOnlyCollection<PluginRecord> Records;
}