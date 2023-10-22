using Orleans.Runtime;
using StreamProcessing.Silo.Interfaces;

namespace StreamProcessing.Silo;

internal sealed class LocalGrainCoordinator : Grain, ILocalGrainCoordinator
{
    private readonly IPersistentState<HashSet<Guid>> _state;

    public LocalGrainCoordinator(
        [PersistentState(stateName: "localGrainIds", storageName: SiloConsts.StorageName)]
        IPersistentState<HashSet<Guid>> state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
    }

    public async Task Subscribe(Guid localSiloGrainId)
    {
        var ids = _state.State ?? new HashSet<Guid>();
        ids.Add(localSiloGrainId);

        _state.State = ids;
        await _state.WriteStateAsync();
    }

    public async Task UnSubscribe(Guid localSiloGrainId)
    {
        var ids = _state.State ?? new HashSet<Guid>();
        ids.Remove(localSiloGrainId);

        _state.State = ids;
        await _state.WriteStateAsync();
    }

    public Task<IReadOnlyCollection<Guid>> GetAllLocalSiloGrainIds()
    {
        IReadOnlyCollection<Guid> ids = _state.State;
        return Task.FromResult(ids);
    }
}