namespace StreamProcessing.Scenario.Domain;

[Immutable]
public record struct ScenarioConfig(Guid Id, IReadOnlyCollection<PluginConfig> Configs, IReadOnlyCollection<LinkConfig> Relations);