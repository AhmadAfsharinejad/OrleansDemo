using Orleans.Concurrency;
using Orleans.Placement;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.HttpListener;

[KeepAlive]
[PreferLocalPlacement]
internal sealed class HttpListenerLocalGrain : Grain, IHttpListenerLocalGrain
{
    private readonly IGrainFactory _grainFactory;
    private readonly IPluginConfigFetcher<HttpListenerConfig> _pluginConfigFetcher;
    private readonly IHttpListenerService _httpListenerService;
    private readonly IHttpListenerOutputFieldTypeGetter _httpListenerOutputFieldTypeGetter;

    public HttpListenerLocalGrain(IGrainFactory grainFactory,
        IPluginConfigFetcher<HttpListenerConfig> pluginConfigFetcher,
        IHttpListenerService httpListenerService,
        IHttpListenerOutputFieldTypeGetter httpListenerOutputFieldTypeGetter)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _httpListenerService = httpListenerService ?? throw new ArgumentNullException(nameof(httpListenerService));
        _httpListenerOutputFieldTypeGetter = httpListenerOutputFieldTypeGetter ?? throw new ArgumentNullException(nameof(httpListenerOutputFieldTypeGetter));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"HttpListenerLocalGrain Activated {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Start([Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);

        var outPluginContext = pluginContext with { InputFieldTypes = _httpListenerOutputFieldTypeGetter.GetOutputs(config) };

        await foreach (var recordListenerContextTuple in _httpListenerService.Listen(config, cancellationToken.CancellationToken))
        {
            var grain = _grainFactory.GetGrain<IHttpListenerResponseLocalGrain>(Guid.NewGuid());
            await grain.CallOutput(outPluginContext, recordListenerContextTuple.Record, recordListenerContextTuple.HttpListenerContext, cancellationToken);
        }
    }
}