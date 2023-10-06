using Orleans.Concurrency;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.RandomGenerator.Interfaces;

namespace StreamProcessing.RandomGenerator;

[StatelessWorker]
internal sealed class RandomGeneratorGrain : PluginGrain<RandomGeneratorConfig>, IRandomGeneratorGrain
{
    private readonly IRandomRecordCreator _randomRecordCreator;

    public RandomGeneratorGrain(IPluginGrainFactory pluginGrainFactory, IRandomRecordCreator randomRecordCreator) 
        : base(pluginGrainFactory)
    {
        _randomRecordCreator = randomRecordCreator ?? throw new ArgumentNullException(nameof(randomRecordCreator));
    }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"RandomGeneratorGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }
    
    [ReadOnly]
    public async Task Compute(Immutable<PluginExecutionContext> pluginContext,
        Immutable<PluginRecords>? pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        var config = await GetConfig(pluginContext.Value.ScenarioId, pluginContext.Value.PluginId);
        
        var records = new List<PluginRecord>(config.BatchCount);

        var columnTypeByName = config.Columns.ToDictionary(x => x.Name, y => y.Type);

        var outPluginContext = pluginContext.Value with { InputFieldTypes = config.Columns.ToDictionary(x => x.Name, y => y.FieldType) };
        
        for (int i = 0; i < config.Count; i++)
        {
            records.Add(_randomRecordCreator.Create(columnTypeByName));

            if (config.BatchCount == records.Count)
            {
                await CallOutputs(outPluginContext, records, cancellationToken);
                records = new List<PluginRecord>(config.BatchCount);
            }
        }

        await CallOutputs(outPluginContext, records, cancellationToken);
    }
}