using System.Net;
using Orleans.Placement;
using Orleans.Runtime;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

#pragma warning disable CS4014
#pragma warning disable CS1998

namespace StreamProcessing.HttpListener;

[PreferLocalPlacement]
internal sealed class HttpListenerResponseLocalGrain : Grain, IHttpListenerResponseLocalGrain
{
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private HttpListenerContext? _httpListenerContext;

    public HttpListenerResponseLocalGrain(IPluginOutputCaller pluginOutputCaller)
    {
        _pluginOutputCaller = pluginOutputCaller ?? throw new ArgumentNullException(nameof(pluginOutputCaller));
    }

    public async Task CallOutput([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord record,
        [Immutable] HttpListenerContext httpListenerContext,
        GrainCancellationToken cancellationToken)
    {
        _httpListenerContext = httpListenerContext;

        RequestContext.Set(HttpListenerConsts.ListenerGrainId, this.GetPrimaryKey());
        
        //Dont await response
        _pluginOutputCaller.CallOutputs(pluginContext, record, cancellationToken);
    }

    public async Task SetResponse([Immutable] HttpResponseTuple responseTuple,
        GrainCancellationToken cancellationToken)
    {
        var response = _httpListenerContext!.Response;

        foreach (var header in responseTuple.Headers)
        {
            response.AddHeader(header.Key, header.Value);
        }

        if (responseTuple.ContentBytes is not null)
        {
            await using var output = response.OutputStream;
            await output.WriteAsync(responseTuple.ContentBytes);
        }

        response.Close();
        
        //TODO deavtive grain
    }
}