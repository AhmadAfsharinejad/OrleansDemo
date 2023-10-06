using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Scenario.Domain;

public record struct PluginConfig
{
    public Guid Id { get; set; }
    public PluginTypeId PluginTypeId { get; set; }
    public IPluginConfig Config { get; set; }
}