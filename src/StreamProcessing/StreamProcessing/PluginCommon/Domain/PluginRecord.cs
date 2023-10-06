namespace StreamProcessing.PluginCommon.Domain;

internal record struct PluginRecord
{
    public IReadOnlyDictionary<string, object> Record;
}