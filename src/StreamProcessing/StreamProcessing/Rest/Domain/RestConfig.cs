using StreamProcessing.HttpListener.Domain;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Rest.Domain;

[Immutable]
public record struct RestConfig : IPluginConfig
{
    public HttpMethod HttpMethod { get; set; }
    public RecordJoinType JoinType { get; set; }
    public string Uri { get; set; }
    public IReadOnlyCollection<HeaderField>? RequestHeaders { get; set; }
    public IReadOnlyCollection<KeyValuePair<string, string>>? RequestStaticHeaders { get; set; }
    public IReadOnlyCollection<QueryStringField>? QueryStrings { get; set; }
    public IReadOnlyCollection<KeyValuePair<string, string>>? StaticQueryStrings { get; set; }
    /// <summary>
    /// We replace ContentFields using string.Format
    /// For example Conent is like: { "age": {0}, "name": {1}}
    /// </summary>
    public string? Content { get; set; }
    public IReadOnlyCollection<string>? ContentFields { get; set; }
    public string? StatusFieldName { get; set; }
    public string? ResponseContentFieldName { get; set; }
    public IReadOnlyCollection<HeaderField>? ResponseHeaders { get; set; }
}