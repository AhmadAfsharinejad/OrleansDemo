using Orleans.Concurrency;
using StreamProcessing.DummyOutput.Domain;
using StreamProcessing.DummyOutput.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.DummyOutput;

[StatelessWorker]
[Reentrant]
internal sealed class DummyOutputGrain : PluginGrain, IDummyOutputGrain
{
    private readonly IPluginConfigFetcher<DummyOutputConfig> _pluginConfigFetcher;
    private int _counter;
    private int _totalCounter;

    public DummyOutputGrain(IPluginConfigFetcher<DummyOutputConfig> pluginConfigFetcher)
    {
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"DummyOutputGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecords pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);

        if (!config.IsWriteEnabled) return;

        _counter += pluginRecords.Records.Count;
        _totalCounter += pluginRecords.Records.Count;

        if (_counter > config.RecordCountInterval)
        {
            Console.WriteLine(_totalCounter);
            Console.WriteLine(pluginRecords.Records.Last());
            _counter = 0;
        }
    }
    
    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord pluginRecord,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);

        if (!config.IsWriteEnabled) return;

        _counter += 1;
        _totalCounter += 1;

        if (_counter > config.RecordCountInterval)
        {
            Console.WriteLine(_totalCounter);
            Console.WriteLine(pluginRecord);
            _counter = 0;
        }
    }
}