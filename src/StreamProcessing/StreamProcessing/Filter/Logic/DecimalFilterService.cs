using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Logic;

internal abstract class DecimalFilterService : IGraterOrLessFilterService
{
    public abstract FieldType FieldType { get; }

    public bool IsEqual(object? firstValue, object? secondValue)
    {
        if (firstValue is null && secondValue is null) return true;
        if (firstValue is null || secondValue is null) return false;
        
        return firstValue.Equals(secondValue);
    }

    public bool IsLess(object? firstValue, object? secondValue)
    {
        if (firstValue is null && secondValue is null) return false;

        if (TryParse(firstValue , out var firstParse) && 
            TryParse(secondValue , out var secondParse) )
        {
            return firstParse < secondParse;
        }

        return false;
    }

    public bool IsGreater(object? firstValue, object? secondValue)
    {
        if (firstValue is null && secondValue is null) return false;

        if (TryParse(firstValue , out var firstParse) && 
            TryParse(secondValue , out var secondParse) )
        {
            return firstParse > secondParse;
        }

        return false;
    }

    private static bool TryParse(object? value, out decimal result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }
        
        if (value is decimal castValue)
        {
            result = castValue;
            return true;
        }

        if (decimal.TryParse(value?.ToString(), out var parsedValue))
        {
            result = parsedValue;
            return true;
        }

        result = default;
        return false;
    }

}