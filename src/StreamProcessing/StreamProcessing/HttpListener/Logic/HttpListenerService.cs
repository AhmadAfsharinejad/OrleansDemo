using System.Net;
using System.Runtime.CompilerServices;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpListener.Logic;

internal sealed class HttpListenerService : IHttpListenerService
{
    public async IAsyncEnumerable<RecordListenerContextTuple> Listen(HttpListenerConfig config, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var listener = new System.Net.HttpListener();
        listener.Prefixes.Add(config.Uri);
        listener.Start();

        while (listener.IsListening 
               && !cancellationToken.IsCancellationRequested)
        {
            var httpListenerContext = await listener.GetContextAsync();
            
            var record = await GetRecord(config, httpListenerContext.Request);

            yield return new RecordListenerContextTuple(httpListenerContext, record);
        }
    }

    private static async Task<PluginRecord> GetRecord(HttpListenerConfig config, HttpListenerRequest request)
    {
        var record = new Dictionary<string, object>();

        if (config.Headers is not null)
        {
            foreach (var header in config.Headers)
            {
                record[header.FieldName] = request.Headers[header.NameInHeader]!;
            }
        }

        if (config.QueryStrings is not null)
        {
            foreach (var queryString in config.QueryStrings)
            {
                record[queryString.FieldName] = request.QueryString[queryString.NameInQueryString]!;
            }
        }

        if (!string.IsNullOrWhiteSpace(config.ConetentFieldName))
        {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            record[config.ConetentFieldName] = await reader.ReadToEndAsync();
        }

        return new PluginRecord(record);
    }
}