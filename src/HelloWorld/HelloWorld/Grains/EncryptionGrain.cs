using HelloWorld.Grains.Domains.States;
using HelloWorld.Grains.Interfaces;
using Orleans.Runtime;

namespace HelloWorld.Grains;

public class EncryptionGrain : Grain, IEncryptionGrain
{
    private readonly IPersistentState<EncryptionState> _state;

    public EncryptionGrain(
        [PersistentState(stateName: "state", storageName: "EncryptionStorage")]
        IPersistentState<EncryptionState> state)
    {
        _state = state;
    }

    public async Task Encrypt(string value)
    {
        _state.State = new()
        {
            Id = this.GetPrimaryKeyString(),
            Value = value
        };

        await _state.WriteStateAsync();
    }

    public Task<string> Decrypt()
    {
        return Task.FromResult(_state.State.Value);
    }
}