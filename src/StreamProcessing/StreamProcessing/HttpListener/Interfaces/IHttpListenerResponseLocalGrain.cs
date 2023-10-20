using System.Net;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpListener.Interfaces;

/// <summary>
/// This grain wait for call from HttpresponseGrain
/// </summary>
internal interface IHttpListenerResponseLocalGrain : IGrainWithGuidKey
{
    Task CallOutput([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord record,
        [Immutable] HttpListenerContext httpListenerContext,
        GrainCancellationToken cancellationToken);

    Task SetResponse([Immutable] HttpResponseTuple responseTuple,
        GrainCancellationToken cancellationToken);
}