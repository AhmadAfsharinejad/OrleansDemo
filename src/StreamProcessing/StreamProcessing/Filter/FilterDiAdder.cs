using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.Filter.Interfaces;
using StreamProcessing.Filter.Logic;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.Filter;

internal sealed class FilterDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, FilterGrainIntroducer>();
        collection.AddSingleton<IFilterService, FilterService>();
        collection.AddSingleton<IDataTypeFilterService, BoolFilterService>();
        collection.AddSingleton<IDataTypeFilterService, DateFilterService>();
        collection.AddSingleton<IDataTypeFilterService, DateTimeFilterService>();
        collection.AddSingleton<IDataTypeFilterService, FloatFilterService>();
        collection.AddSingleton<IDataTypeFilterService, GuidFilterService>();
        collection.AddSingleton<IDataTypeFilterService, IntegerFilterService>();
        collection.AddSingleton<IDataTypeFilterService, TextFilterService>();
        collection.AddSingleton<IDataTypeFilterService, TimeSpanFilterService>();
    }
}