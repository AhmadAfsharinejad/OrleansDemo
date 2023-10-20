using Orleans.Services;

namespace StreamProcessing.Silo.Interfaces;

/// <summary>
/// Create a local silo (LocalSiloGrain) and subscribe it to LocalGrainCoordinator
/// </summary>
internal interface ILocalGrainIntroducerService : IGrainService
{
}