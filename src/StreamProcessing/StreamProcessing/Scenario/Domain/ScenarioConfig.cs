namespace StreamProcessing.Scenario.Domain;

public record struct ScenarioConfig
{
    public Guid Id { get; set; }
    public IReadOnlyCollection<PluginConfig> Configs { get; set; }
    public IReadOnlyCollection<LinkConfig> Relations { get; set; }
}