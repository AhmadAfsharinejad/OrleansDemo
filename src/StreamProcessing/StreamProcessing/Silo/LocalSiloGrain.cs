using Orleans.Placement;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Silo.Interfaces;

namespace StreamProcessing.Silo;

[PreferLocalPlacement]
internal sealed class LocalSiloGrain : Grain, ILocalSiloGrain
{
    private readonly IGrainFactory _grainFactory;
    private readonly IPluginGrainFactory _pluginGrainFactory;

    public LocalSiloGrain(IGrainFactory grainFactory, IPluginGrainFactory pluginGrainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        _pluginGrainFactory = pluginGrainFactory ?? throw new ArgumentNullException(nameof(pluginGrainFactory));
    }
    
    public async Task SubscribeToMasterGrain()
    {
        var masterGrain = _grainFactory.GetGrain<ILocalGrainCoordinator>(SiloConsts.MasterGrainId);
        await masterGrain.Subscribe(this.GetPrimaryKey());
    }

    public async Task UnSubscribeToMasterGrain()
    {
        var masterGrain = _grainFactory.GetGrain<ILocalGrainCoordinator>(SiloConsts.MasterGrainId);
        await masterGrain.UnSubscribe(this.GetPrimaryKey());
    }

    public async Task StartPlugin([Immutable] Type startingPluginType, 
        [Immutable] PluginExecutionContext pluginContext, 
        GrainCancellationToken cancellationToken)
    {
        var grain = _pluginGrainFactory.GetOrCreateSourcePlugin(startingPluginType, Guid.NewGuid());
        await grain.Start(pluginContext, cancellationToken);
    }
}