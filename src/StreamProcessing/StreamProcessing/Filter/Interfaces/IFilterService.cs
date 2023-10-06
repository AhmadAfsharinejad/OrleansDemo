using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Interfaces;

internal interface IFilterService
{
    bool Satisfy(PluginRecord pluginRecord, IConstraint filterConstraint, IReadOnlyDictionary<string, FieldType> inputFieldTypes);
}