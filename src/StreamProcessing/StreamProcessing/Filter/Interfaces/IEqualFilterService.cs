namespace StreamProcessing.Filter.Interfaces;

internal interface IEqualFilterService : IDataTypeFilterService
{
    bool IsEqual(object? firstValue, object? secondValue);
}