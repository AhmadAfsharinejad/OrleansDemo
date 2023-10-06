using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Scenario.Domain;

[Immutable]
public record struct PluginConfig(PluginTypeId PluginTypeId, Guid Id, IPluginConfig Config);
