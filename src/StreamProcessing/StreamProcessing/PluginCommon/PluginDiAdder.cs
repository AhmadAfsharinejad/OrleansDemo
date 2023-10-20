using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.PluginCommon.Logic;

namespace StreamProcessing.PluginCommon;

public class PluginDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainFactory, PluginGrainFactory>();
        collection.AddTransient(typeof(IPluginConfigFetcher<>), typeof(PluginConfigFetcher<>));
        collection.AddTransient<IPluginOutputCaller, PluginOutputCaller>();
        collection.AddSingleton<IRecordJoiner, RecordJoiner>();
        collection.AddSingleton<IFieldTypeJoiner, FieldTypeJoiner>();
        collection.AddSingleton<IStringReplacer, StringReplacer>();
    }
}