namespace StreamProcessing.PluginCommon.Domain;

internal record struct PluginRecord(IReadOnlyDictionary<string, object> Record);
