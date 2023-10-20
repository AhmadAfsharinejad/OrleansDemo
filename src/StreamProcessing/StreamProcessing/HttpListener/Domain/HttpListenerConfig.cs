using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpListener.Domain;

[Immutable]
public record struct HttpListenerConfig : IPluginConfig
{
    public string Url { get; set; }
    public IReadOnlyCollection<HeaderField>? Headers { get; set; }
    public IReadOnlyCollection<QueryStringField>? QueryStrings { get; set; }
    public string? ConetentFieldName { get; set; }
}