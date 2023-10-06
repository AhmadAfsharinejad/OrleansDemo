using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.RandomGenerator.Domain;

[Immutable]
public record struct RandomGeneratorConfig : IPluginConfig
{
    public RandomGeneratorConfig()
    {
        BatchCount = 10;
        Columns = Array.Empty<RandomColumn>();
    }

    public short BatchCount { get; set; }
    public long Count { get; set; }
    public IReadOnlyCollection<RandomColumn> Columns { get; set; }
}