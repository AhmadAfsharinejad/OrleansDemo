using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Map.Domain;

[Immutable]
public record struct MapConfig : IPluginConfig
{
    public IReadOnlyCollection<StreamField> OutputColumns { get; set; }
    public string Code { get; set; }
    public string FullClassName { get; set; }
    public string FunctionName { get; set; }
}