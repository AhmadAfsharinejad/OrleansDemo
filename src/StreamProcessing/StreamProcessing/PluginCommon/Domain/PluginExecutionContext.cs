namespace StreamProcessing.PluginCommon.Domain;

[Immutable]
internal record struct PluginExecutionContext(Guid ScenarioId, Guid PluginId, Dictionary<string, FieldType>? InputFieldTypes);