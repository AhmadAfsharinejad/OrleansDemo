using Orleans.Concurrency;
using StreamProcessing.DummyOutput.Domain;
using StreamProcessing.DummyOutput.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.DummyOutput;

[StatelessWorker]
internal sealed class DummyOutputGrain : PluginGrain<DummyOutputConfig>, IDummyOutputGrain
{
    private int _counter;
    private int _totalCounter;

    public DummyOutputGrain(IPluginGrainFactory pluginGrainFactory) : base(pluginGrainFactory)
    {
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"DummyOutputGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    //[OneWay]
    public async Task Compute(Guid scenarioId, Guid pluginId,
        Immutable<PluginRecords>? pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        if (pluginRecords is null) return;

        var config = await GetConfig(scenarioId, pluginId);

        if (!config.IsWriteEnabled) return;

        _counter += pluginRecords.Value.Value.Records.Count;
        _totalCounter += pluginRecords.Value.Value.Records.Count;

        if (_counter > config.RecordCountInterval)
        {
            Console.WriteLine(_totalCounter);
            _counter = 0;
        }
    }
}