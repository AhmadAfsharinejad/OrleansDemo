using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Scenario.Interfaces;

namespace StreamProcessing.PluginCommon.Logic;

internal sealed class PluginOutputCaller : IPluginOutputCaller
{
    private readonly IGrainFactory _grainFactory;
    private readonly IPluginGrainFactory _pluginGrainFactory;
    private IReadOnlyCollection<PluginTypeWithId>? _outputs;

    public PluginOutputCaller(IGrainFactory grainFactory, IPluginGrainFactory pluginGrainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        _pluginGrainFactory = pluginGrainFactory ?? throw new ArgumentNullException(nameof(pluginGrainFactory));
    }
    
    public async Task CallOutputs(PluginExecutionContext pluginContext, 
        List<PluginRecord> records, 
        GrainCancellationToken cancellationToken)
    {
        var outputs = await GetOutpus(pluginContext.ScenarioId, pluginContext.PluginId);
        if (outputs.Count == 0) return;

        var outputRecords = new PluginRecords { Records = records };
        
        var tasks = new List<Task>(outputs.Count);

        foreach (var output in outputs)
        {
            var outPluginContext = pluginContext with { PluginId = output.PluginId };
            var pluginGrain = _pluginGrainFactory.GetOrCreatePlugin(output.PluginTypeId, output.PluginId);
            var task = pluginGrain.Compute(outPluginContext, outputRecords, cancellationToken);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    public async Task CallOutputs(PluginExecutionContext pluginContext,
        PluginRecord record, 
        GrainCancellationToken cancellationToken)
    {
        var outputs = await GetOutpus(pluginContext.ScenarioId, pluginContext.PluginId);
        if (outputs.Count == 0) return;
        
        var tasks = new List<Task>(outputs.Count);

        foreach (var output in outputs)
        {
            var outPluginContext = pluginContext with { PluginId = output.PluginId };
            var pluginGrain = _pluginGrainFactory.GetOrCreatePlugin(output.PluginTypeId, output.PluginId);
            var task = pluginGrain.Compute(outPluginContext, record, cancellationToken);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private async Task<IReadOnlyCollection<PluginTypeWithId>> GetOutpus(Guid scenarioId, Guid pluginId)
    {
        if (_outputs is not null) return _outputs;
        
        var scenarioGrain = _grainFactory.GetGrain<IScenarioGrain>(scenarioId);
        _outputs = await scenarioGrain.GetOutputTypes(pluginId);
        return _outputs;
    }
}