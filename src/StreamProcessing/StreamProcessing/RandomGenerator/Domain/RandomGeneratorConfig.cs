using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.RandomGenerator.Domain;

public record struct RandomGeneratorConfig : IPluginConfig
{
    public long Count { get; set; }
}