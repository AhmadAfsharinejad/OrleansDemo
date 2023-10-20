namespace StreamProcessing.PluginCommon.Domain;

internal record struct PluginRecord(IReadOnlyDictionary<string, object> Record)
{
    public override string ToString()
    {
        return string.Join(",", Record.Select(x => $"{x.Key}:{x.Value}"));
    }
}
