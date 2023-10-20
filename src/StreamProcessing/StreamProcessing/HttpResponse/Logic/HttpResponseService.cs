using System.Text;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.HttpResponse.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpResponse.Logic;

internal sealed class HttpResponseService : IHttpResponseService
{
    private readonly IStringReplacer _stringReplacer;

    public HttpResponseService(IStringReplacer stringReplacer)
    {
        _stringReplacer = stringReplacer ?? throw new ArgumentNullException(nameof(stringReplacer));
    }
    
    public HttpResponseTuple GetResponse(HttpResponseConfig config, PluginRecord record)
    {
        return new HttpResponseTuple(GetContent(config, record), GetHeaders(config, record));
    }

    private static IReadOnlyCollection<KeyValuePair<string, string>> GetHeaders(HttpResponseConfig config, PluginRecord record)
    {
        List<KeyValuePair<string, string>> headers = new();

        if (config.StaticHeaders is not null)
        {
            foreach (var staticHeader in config.StaticHeaders)
            {
                headers.Add(new KeyValuePair<string, string>(staticHeader.Key, staticHeader.Value));
            }
        }

        if (config.Headers is null)
        {
            return headers;
        }

        foreach (var header in config.Headers)
        {
            headers.Add(new KeyValuePair<string, string>(header.NameInHeader, record.Record[header.FieldName].ToString()!));
        }

        return headers;
    }

    private byte[]? GetContent(HttpResponseConfig config, PluginRecord record)
    {
        byte[]? content = null;
        var stringContent = _stringReplacer.Replace(config.Content, config.ContentFields, record);
        if (!string.IsNullOrWhiteSpace(stringContent))
        {
            content = Encoding.UTF8.GetBytes(stringContent);
        }

        return content;
    }
}