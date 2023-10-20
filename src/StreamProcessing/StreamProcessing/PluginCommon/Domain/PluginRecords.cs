namespace StreamProcessing.PluginCommon.Domain;

[Immutable]
internal record struct PluginRecords(IReadOnlyCollection<PluginRecord> Records);
