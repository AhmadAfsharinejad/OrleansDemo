using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Logic;

public class GuidFilterService : IEqualFilterService
{
    public FieldType FieldType => FieldType.Guid;

    public bool IsEqual(object? firstValue, object? secondValue)
    {
        if (firstValue is null && secondValue is null) return true;
        if (firstValue is null || secondValue is null) return false;
        
        return firstValue.Equals(secondValue);
    }
}