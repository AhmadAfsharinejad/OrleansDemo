using Orleans.Concurrency;
using StreamProcessing.Map.Domain;
using StreamProcessing.Map.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Map;

[StatelessWorker]
[Reentrant]
internal sealed class MapGrain : PluginGrain, IMapGrain
{
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<MapConfig> _pluginConfigFetcher;
    private readonly ICompiler _compiler;
    private bool _hasBeenInitialized;
    private IReadOnlyDictionary<string, FieldType>? _outputFieldTypes;
    private Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>? _func;

    public MapGrain(IPluginOutputCaller pluginOutputCaller,
        IPluginConfigFetcher<MapConfig> pluginConfigFetcher,
        ICompiler compiler)
    {
        _pluginOutputCaller = pluginOutputCaller ?? throw new ArgumentNullException(nameof(pluginOutputCaller));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"MapGrain Activated {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecords pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        await Init(pluginContext);

        var records = new List<PluginRecord>(pluginRecords.Records.Count);

        foreach (var pluginRecord in pluginRecords.Records)
        {
            var resultRecord = _func!(pluginRecord.Record);
            records.Add(new PluginRecord(resultRecord));
        }

        await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), records, cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord pluginRecord,
        GrainCancellationToken cancellationToken)
    {
        await Init(pluginContext);

        var resultRecord = _func!(pluginRecord.Record);

        await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext),
            new PluginRecord(resultRecord),
            cancellationToken);
    }

    private async Task Init(PluginExecutionContext pluginContext)
    {
        if (_hasBeenInitialized) return;

        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);

        _func = _compiler.CreateFunction(config.Code, config.FullClassName, config.FunctionName);

        _outputFieldTypes = config.OutputColumns.ToDictionary(x => x.Name, y => y.Type);

        _hasBeenInitialized = true;
    }

    private PluginExecutionContext GetOutPluginContext(PluginExecutionContext pluginContext)
    {
        return pluginContext with { InputFieldTypes = _outputFieldTypes };
    }
}