namespace StreamProcessing.Silo.Interfaces;

/// <summary>
/// All Silo must introduce one grain (ILocalSiloGrain) to this this grain
/// using introduced grain for runnig source plugins in each silo
/// </summary>
internal interface ILocalGrainCoordinator : IGrainWithGuidKey
{
    Task Subscribe(Guid grainId);
    Task UnSubscribe(Guid grainId);
    Task<IReadOnlyCollection<Guid>> GetAllLocalSiloGrainIds();
}