using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Aggregation.Domain;

[Immutable]
public record struct AggregationConfig : IPluginConfig
{
    public string FieldName { get; set; }
    public FieldType AggregateType { get; set; }
    public AggregationOperators Operator { get; set; }
}