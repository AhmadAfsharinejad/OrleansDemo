using Orleans.Concurrency;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.RandomGenerator.Interfaces;

namespace StreamProcessing.RandomGenerator;

[StatelessWorker]
[Reentrant]
//TODO chechout https://github.com/dotnet/orleans/blob/main/test/Grains/BenchmarkGrains/Transaction/LoadGrain.cs for stream
internal sealed class RandomGeneratorGrain : PluginGrain, IRandomGeneratorGrain
{
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<RandomGeneratorConfig> _pluginConfigFetcher;
    private readonly IRandomRecordCreator _randomRecordCreator;

    public RandomGeneratorGrain(IPluginOutputCaller pluginOutputCaller,
        IPluginConfigFetcher<RandomGeneratorConfig> pluginConfigFetcher,
        IRandomRecordCreator randomRecordCreator)
    {
        _pluginOutputCaller = pluginOutputCaller ?? throw new ArgumentNullException(nameof(pluginOutputCaller));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _randomRecordCreator = randomRecordCreator ?? throw new ArgumentNullException(nameof(randomRecordCreator));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        //TODO change console with log
        Console.WriteLine($"RandomGeneratorGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Start([Immutable]PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);

        var records = new List<PluginRecord>(config.BatchCount);

        var columnTypeByName = config.Columns.ToDictionary(x => x.Field.Name, y => y.Type);

        var outPluginContext = pluginContext with { InputFieldTypes = config.Columns.ToDictionary(x => x.Field.Name, y => y.Field.Type) };

        for (int i = 0; i < config.Count; i++)
        {
            records.Add(_randomRecordCreator.Create(columnTypeByName));

            if (config.BatchCount == records.Count)
            {
                await TryCallOutputs(outPluginContext, records, cancellationToken);

                records = new List<PluginRecord>(config.BatchCount);
            }
        }

        await TryCallOutputs(outPluginContext, records, cancellationToken);
    }

    private async Task TryCallOutputs(PluginExecutionContext pluginContext,
        List<PluginRecord> records,
        GrainCancellationToken cancellationToken)
    {
        //TODO .Ignore() ?
        try
        {
            await _pluginOutputCaller.CallOutputs(pluginContext, records, cancellationToken);
        }
        catch (Exception e)
        {
            //TODO what to do?
            Console.WriteLine(e);
        }
    }
}