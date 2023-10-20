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

    public HttpListenerLocalGrain(IGrainFactory grainFactory,
        IPluginConfigFetcher<HttpListenerConfig> pluginConfigFetcher,
        IHttpListenerService httpListenerService)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _httpListenerService = httpListenerService ?? throw new ArgumentNullException(nameof(httpListenerService));
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

        var outPluginContext = pluginContext with { InputFieldTypes = GetOutputs(config) };
        
        await foreach (var recordListenerContextTuple in _httpListenerService.Listen(config, cancellationToken.CancellationToken))
        {
            var grain = _grainFactory.GetGrain<IHttpListenerResponseLocalGrain>(Guid.NewGuid());
            await grain.CallOutput(outPluginContext, recordListenerContextTuple.Record, recordListenerContextTuple.HttpListenerContext, cancellationToken);
        }
    }

    private static IReadOnlyDictionary<string, FieldType> GetOutputs(HttpListenerConfig config)
    {
        var outputs = new Dictionary<string, FieldType>();

        if (config.Headers is not null)
        {
            foreach (var header in config.Headers)
            {
                outputs[header.FieldName] = FieldType.Text;
            }
        }
        
        if (config.QueryStrings is not null)
        {
            foreach (var queryStrings in config.QueryStrings)
            {
                outputs[queryStrings.FieldName] = FieldType.Text;
            }
        }
        
        if (!string.IsNullOrWhiteSpace(config.ConetentFieldName))
        {
            outputs[config.ConetentFieldName] = FieldType.Text;
        }

        return outputs;
    }
}