using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Scenario.Domain;
using StreamProcessing.Scenario.Interfaces;

namespace StreamProcessing.Scenario;

//TODO add stop - cancellation token
internal class ScenarioRunner : IScenarioRunner
{
    private readonly IGrainFactory _grainFactory;
    private readonly IPluginGrainFactory _pluginGrainFactory;

    public ScenarioRunner(IGrainFactory grainFactory, IPluginGrainFactory pluginGrainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        _pluginGrainFactory = pluginGrainFactory ?? throw new ArgumentNullException(nameof(pluginGrainFactory));
    }

    public async Task Run(ScenarioConfig config)
    {
        using var tcs = new GrainCancellationTokenSource();
        
        var runTasks = new List<Task>();

        foreach (var plugin in FindStartingPlugins(config))
        {
            var grain = _pluginGrainFactory.GetOrCreate(plugin.PluginTypeId, plugin.Id);
            var runTask = grain.Compute(plugin.Id.AsImmutable(), null, tcs.Token);
            runTasks.Add(runTask);
        }

        await Task.WhenAll(runTasks);
    }

    private static IEnumerable<PluginConfig> FindStartingPlugins(ScenarioConfig config)
    {
        var destinationIds = config.Relations.Select(x => x.DestinationId).ToHashSet();
        return config.Configs.Where(x => !destinationIds.Contains(x.Id));
    }
}