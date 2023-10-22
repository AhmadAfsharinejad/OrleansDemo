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
    
    public async Task SubscribeToCoordinator()
    {
        var coordinatorGrain = _grainFactory.GetGrain<ILocalGrainCoordinator>(SiloConsts.CoordinatorGrainId);
        await coordinatorGrain.Subscribe(this.GetPrimaryKey());
    }

    public async Task UnSubscribeToCoordinator()
    {
        var coordinatorGrain = _grainFactory.GetGrain<ILocalGrainCoordinator>(SiloConsts.CoordinatorGrainId);
        await coordinatorGrain.UnSubscribe(this.GetPrimaryKey());
    }

    public async Task StartPlugin([Immutable] Type startingPluginType, 
        [Immutable] PluginExecutionContext pluginContext, 
        GrainCancellationToken cancellationToken)
    {
        var grain = _pluginGrainFactory.GetOrCreateSourcePlugin(startingPluginType, Guid.NewGuid());
        await grain.Start(pluginContext, cancellationToken);
    }
}