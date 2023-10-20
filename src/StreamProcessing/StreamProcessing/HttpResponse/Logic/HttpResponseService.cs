using System.Text;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.HttpResponse.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpResponse.Logic;

internal sealed class HttpResponseService : IHttpResponseService
{
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

    private static byte[]? GetContent(HttpResponseConfig config, PluginRecord record)
    {
        byte[]? content = null;
        var stringContent = GetStringContent(config, record);
        if (!string.IsNullOrWhiteSpace(stringContent))
        {
            content = Encoding.UTF8.GetBytes(stringContent);
        }

        return content;
    }

    private static string? GetStringContent(HttpResponseConfig config, PluginRecord record)
    {
        if (string.IsNullOrWhiteSpace(config.Content)) return null;

        if (config.ContentFields is null || config.ContentFields.Count == 0) return config.Content;

        var args = new object[config.ContentFields.Count];
        var index = 0;
        foreach (var contentField in config.ContentFields)
        {
            args[index++] = record.Record[contentField];
        }

        return string.Format(config.Content, args);
    }
}