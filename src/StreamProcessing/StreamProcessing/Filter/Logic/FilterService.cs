using StreamProcessing.Filter.Domain;
using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Logic;

internal sealed class FilterService : IFilterService
{
    private readonly Dictionary<FieldType, IDataTypeFilterService> _filterServicesByType;

    public FilterService(IEnumerable<IDataTypeFilterService> filters)
    {
        _filterServicesByType = filters.ToDictionary(x => x.FieldType);
    }

    public bool Satisfy(PluginRecord pluginRecord, IConstraint filterConstraint, IReadOnlyDictionary<string, FieldType> inputFieldTypes)
    {
        if (filterConstraint is FieldConstraint fieldConstraint)
        {
            return Satisfy(pluginRecord, fieldConstraint, inputFieldTypes);
        }

        var logicalConstraint = (LogicalConstraint)filterConstraint;

        if (logicalConstraint.Operator == ConstraintOperator.And)
        {
            return logicalConstraint.Constraints.All(x => Satisfy(pluginRecord, x, inputFieldTypes));
        }

        return logicalConstraint.Constraints.Any(x => Satisfy(pluginRecord, x, inputFieldTypes));
    }

    private bool Satisfy(PluginRecord pluginRecord,
        FieldConstraint fieldConstraint,
        IReadOnlyDictionary<string, FieldType> inputFieldTypes)
    {
        if (!pluginRecord.Record.TryGetValue(fieldConstraint.FieldName, out var recordValue))
        {
            return true;
        }

        if (!inputFieldTypes.TryGetValue(fieldConstraint.FieldName, out var fieldType))
        {
            throw new Exception($"Column '{fieldConstraint}' not exist in input schema.");
        }

        if (!_filterServicesByType.TryGetValue(fieldType, out var service))
        {
            throw new Exception($"There is no filter service for type '{fieldType}'.");
        }

        if (fieldConstraint.Operator == ConstraintOperators.Equal)
        {
            if (service is not IEqualFilterService equalFilterService)
            {
                throw new Exception($"Type '{fieldType}' doesn't support 'equal' operator.");
            }

            return equalFilterService.IsEqual(recordValue, fieldConstraint.Value);
        }
        
        if (fieldConstraint.Operator == ConstraintOperators.Less)
        {
            if (service is not IGraterOrLessFilterService lessFilterService)
            {
                throw new Exception($"Type '{fieldType}' doesn't support 'less' operator.");
            }

            return lessFilterService.IsLess(recordValue, fieldConstraint.Value);
        }
        
        if (fieldConstraint.Operator == ConstraintOperators.Greater)
        {
            if (service is not IGraterOrLessFilterService greaterFilterService)
            {
                throw new Exception($"Type '{fieldType}' doesn't support 'greater' operator.");
            }

            return greaterFilterService.IsGreater(recordValue, fieldConstraint.Value);
        }


        throw new NotSupportedException($"there is no handler for '{fieldConstraint.Operator}' operator");
    }
}