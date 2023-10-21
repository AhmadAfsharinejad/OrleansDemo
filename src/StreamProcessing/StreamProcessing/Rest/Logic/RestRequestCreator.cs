using System.Text;
using System.Web;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest.Logic;

internal sealed class RestRequestCreator : IRestRequestCreator
{
    private readonly IStringReplacer _stringReplacer;
    private readonly IUriReplacer _uriReplacer;

    public RestRequestCreator(IStringReplacer stringReplacer, IUriReplacer uriReplacer)
    {
        _stringReplacer = stringReplacer ?? throw new ArgumentNullException(nameof(stringReplacer));
        _uriReplacer = uriReplacer ?? throw new ArgumentNullException(nameof(uriReplacer));
    }

    public HttpRequestMessage Create(RestConfig config, PluginRecord pluginRecord, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new(config.HttpMethod, _uriReplacer.GetUri(config, pluginRecord));

        AddHeaders(request, config, pluginRecord);

        AddContent(request, config, pluginRecord);

        return request;
    }

    private void AddContent(HttpRequestMessage request, RestConfig config, PluginRecord pluginRecord)
    {
        var content = _stringReplacer.Replace(config.Content, config.ContentFields, pluginRecord);

        if (string.IsNullOrWhiteSpace(content)) return;

        request.Content = new StringContent(content, Encoding.UTF8);
    }

    private static void AddHeaders(HttpRequestMessage request, RestConfig config, PluginRecord pluginRecord)
    {
        if (config.RequestHeaders is not null)
        {
            foreach (var headerField in config.RequestHeaders)
            {
                request.Headers.Add(headerField.NameInHeader, pluginRecord.Record[headerField.FieldName].ToString());
            }
        }

        if (config.RequestStaticHeaders is not null)
        {
            foreach (var header in config.RequestStaticHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
    }
}