using System.Data;
using Orleans.Concurrency;
using StreamProcessing.Filter.Domain;
using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Filter;

[StatelessWorker]
internal sealed class FilterGrain : PluginGrain<FilterConfig>, IFilterGrain
{
    private readonly IFilterService _filterService;

    public FilterGrain(IPluginGrainFactory pluginGrainFactory, IFilterService filterService) : base(pluginGrainFactory)
    {
        _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
    }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"FilterGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }
    
    [ReadOnly]
    public async Task Compute(Immutable<PluginExecutionContext> pluginContext, 
        Immutable<PluginRecords>? pluginRecords, 
        GrainCancellationToken cancellationToken)
    {
        if(pluginRecords is null) return;

        if (pluginContext.Value.InputFieldTypes is null) throw new NoNullAllowedException("'InputFieldTypes' can't be null.");
        
        var config = await GetConfig(pluginContext.Value.ScenarioId, pluginContext.Value.PluginId);

        var records = new List<PluginRecord>(pluginRecords.Value.Value.Records.Count);

        foreach (var pluginRecord in pluginRecords.Value.Value.Records!)
        {
            if (_filterService.Satisfy(pluginRecord, config.Constraint, pluginContext.Value.InputFieldTypes!))
            {
                records.Add(pluginRecord);
            }
        }

        await CallOutputs(pluginContext.Value, records, cancellationToken);
    }
}