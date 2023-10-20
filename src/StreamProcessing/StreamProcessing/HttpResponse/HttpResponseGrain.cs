using Orleans.Concurrency;
using Orleans.Runtime;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.HttpResponse.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpResponse;

[StatelessWorker]
[Reentrant]
internal sealed class HttpResponseGrain : PluginGrain, IHttpResponseGrain
{
    private readonly IPluginConfigFetcher<HttpResponseConfig> _pluginConfigFetcher;
    private readonly IHttpResponseService _httpResponseService;
    private readonly IGrainFactory _grainFactory;

    public HttpResponseGrain(IPluginConfigFetcher<HttpResponseConfig> pluginConfigFetcher,
        IHttpResponseService httpResponseService,
        IGrainFactory grainFactory)
    {
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _httpResponseService = httpResponseService ?? throw new ArgumentNullException(nameof(httpResponseService));
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"HttpResponseGrain Activated {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord pluginRecord,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);

        var response = _httpResponseService.GetResponse(config, pluginRecord);

        var listenerGrainId = (Guid)RequestContext.Get(HttpListenerConsts.ListenerGrainId);

        var grain = _grainFactory.GetGrain<IHttpListenerResponseLocalGrain>(listenerGrainId);
        await grain.SetResponse(response, cancellationToken);
    }

    [ReadOnly]
    public Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecords pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        //TODO DELETE
        throw new NotImplementedException();
    }
}