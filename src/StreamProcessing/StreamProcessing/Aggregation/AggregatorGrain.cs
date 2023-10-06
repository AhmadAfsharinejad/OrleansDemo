using Orleans.Concurrency;
using StreamProcessing.Aggregation.Domain;
using StreamProcessing.Aggregation.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Aggregation;

[StatelessWorker]
internal sealed class AggregatorGrain : PluginGrain<AggregationConfig>, IAggregatorGrain
{
    public AggregatorGrain(IPluginGrainFactory pluginGrainFactory) : base(pluginGrainFactory)
    {
    }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"AggregatorGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Compute(Immutable<PluginExecutionContext> pluginContext, 
        Immutable<PluginRecords>? pluginRecords, 
        GrainCancellationToken cancellationToken)
    {
        if(pluginRecords is null) return;

        
        throw new NotImplementedException();
    }
}