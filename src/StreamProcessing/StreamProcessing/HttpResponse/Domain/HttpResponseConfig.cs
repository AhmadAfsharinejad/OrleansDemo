using StreamProcessing.HttpListener.Domain;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpResponse.Domain;

[Immutable]
public record struct HttpResponseConfig : IPluginConfig
{
    public IReadOnlyCollection<HeaderField>? Headers { get; set; }
    public IReadOnlyCollection<KeyValuePair<string, string>>? StaticHeaders { get; set; }
    /// <summary>
    /// We replace ContentFields using string.Format
    /// For example Conent is like: { "age": {0}, "name": {1}}
    /// </summary>
    public string? Content { get; set; }
    public IReadOnlyCollection<string>? ContentFields { get; set; }
    //TODO status
}