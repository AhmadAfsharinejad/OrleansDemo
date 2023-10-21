using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using StreamProcessing.Silo.Interfaces;
// ReSharper disable ClassNeverInstantiated.Global

namespace StreamProcessing.Silo;

internal sealed class LocalGrainIntroducerService : GrainService, ILocalGrainIntroducerService
{
    private readonly IGrainFactory _grainFactory;
    private readonly Guid _siloGrainId;

    public LocalGrainIntroducerService(
        GrainId id,
        Orleans.Runtime.Silo silo,
        ILoggerFactory loggerFactory,
        IGrainFactory grainFactory)
        : base(id, silo, loggerFactory)
    {
        _grainFactory = grainFactory;
        _siloGrainId = Guid.NewGuid();
    }

    public override async Task Start()
    {
        var siloGrain = _grainFactory.GetGrain<ILocalSiloGrain>(_siloGrainId);
        await siloGrain.SubscribeToMasterGrain();
        
        await base.Start();
    }

    public override async Task Stop()
    {
        var siloGrain = _grainFactory.GetGrain<ILocalSiloGrain>(_siloGrainId);
        await siloGrain.UnSubscribeToMasterGrain();

        await base.Stop();
    }
}