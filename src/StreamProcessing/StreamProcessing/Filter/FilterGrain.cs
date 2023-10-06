using Orleans.Concurrency;
using StreamProcessing.Filter.Domain;
using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter;

[StatelessWorker]
internal sealed class FilterGrain : PluginGrain<FilterConfig>, IFilterGrain
{
    [ReadOnly]
    [OneWay]
    public async Task Compute(Immutable<Guid> pluginId, Immutable<PluginRecords>? pluginRecords, GrainCancellationToken cancellationToken)
    {
        if(pluginRecords is null) return;
        
        var config = await GetConfig(pluginId.Value);

        foreach (var pluginRecord in pluginRecords.Value.Value.Records!)
        {
            if (Filter(pluginRecord, config.Constraint))
            {
                
            }
        }

        throw new NotImplementedException();
    }

    private bool Filter(PluginRecord pluginRecord, IConstraint filterConstraint)
    {
        if (filterConstraint is FieldConstraint fieldConstraint)
        {
            return Filter(pluginRecord, fieldConstraint);
        }
       
        var logicalConstraint = (LogicalConstraint)filterConstraint;
        
        if (logicalConstraint.Operator == ConstraintOperator.And)
        {
            return logicalConstraint.Constraints.All(x => Filter(pluginRecord, x));
        }

        return logicalConstraint.Constraints.Any(x => Filter(pluginRecord, x));
    }

    private bool Filter(PluginRecord pluginRecord, FieldConstraint fieldConstraint)
    {

        if (!pluginRecord.Record.TryGetValue(fieldConstraint.Key, out var recordValue))
        {
            return true;
        }

        //TODO
        return fieldConstraint.Condition switch
        {
            ConstraintCondition.Equal => recordValue == fieldConstraint.Value,
        };
    }
}