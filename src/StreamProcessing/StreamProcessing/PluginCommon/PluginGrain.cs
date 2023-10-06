using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Scenario.Interfaces;

namespace StreamProcessing.PluginCommon;

//TODO service she
internal abstract class PluginGrain<TConfig> : Grain
{
    private readonly IPluginGrainFactory _pluginGrainFactory;
    private TConfig? _config;
    protected IReadOnlyCollection<PluginTypeWithId>? _outputs;

    protected PluginGrain(IPluginGrainFactory pluginGrainFactory)
    {
        _pluginGrainFactory = pluginGrainFactory ?? throw new ArgumentNullException(nameof(pluginGrainFactory));
    }

    protected async Task<TConfig> GetConfig(Guid scenarioId, Guid pluginId)
    {
        //TODO mishe ba in raft - aggregation moshkel nadare?
        //var pluginId = this.GetPrimaryKey();

        if (!_config.Equals(default(TConfig))) return _config;
        
        var scenarioGrain = GrainFactory.GetGrain<IScenarioGrain>(scenarioId);
        var config = await scenarioGrain.GetPluginConfig(pluginId);

        if (config is not TConfig tConfig) throw new InvalidCastException($"Cant cast plugin '{pluginId}' to specific type.");

        _config = tConfig;
        return _config;
    }
    
    protected async Task CallOutputs(Guid scenarioId, Guid pluginId, List<PluginRecord> records, GrainCancellationToken cancellationToken)
    {
        var outputs = await GetOutpus(scenarioId, pluginId);
        if (outputs.Count == 0) return;

        var outputRecords = new PluginRecords { Records = records }.AsImmutable();
        
        var tasks = new List<Task>(outputs.Count);

        foreach (var output in outputs)
        {
            var pluginGrain = _pluginGrainFactory.GetOrCreate(GrainFactory,output.PluginTypeId, output.PluginId);
            var task = pluginGrain.Compute(scenarioId, output.PluginId, outputRecords, cancellationToken);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private async Task<IReadOnlyCollection<PluginTypeWithId>> GetOutpus(Guid scenarioId, Guid pluginId)
    {
        if (_outputs is not null) return _outputs;
        
        var scenarioGrain = GrainFactory.GetGrain<IScenarioGrain>(scenarioId);
        _outputs = await scenarioGrain.GetOutputTypes(pluginId);
        return _outputs;
    }
}