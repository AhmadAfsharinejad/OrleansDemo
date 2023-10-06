using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Interfaces;

internal interface IDataTypeFilterService
{
    FieldType FieldType { get; } 
}