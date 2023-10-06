namespace StreamProcessing.PluginCommon.Domain;

[Immutable]
internal record struct PluginTypeWithId(Guid PluginId, PluginTypeId PluginTypeId);
