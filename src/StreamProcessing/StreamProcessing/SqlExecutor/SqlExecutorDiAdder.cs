using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.SqlExecutor.Interfaces;
using StreamProcessing.SqlExecutor.Logic;

namespace StreamProcessing.SqlExecutor;

internal sealed class SqlExecutorDiAdder: IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IPluginGrainIntroducer, SqlExecutorGrainIntroducer>();
        collection.AddSingleton<IParameterCommandCreator, ParameterCommandCreator>();
        collection.AddSingleton<IDmlExecutor, DmlExecutor>();
        collection.AddSingleton<IDqlReader, DqlReader>();
        collection.AddSingleton<ISqlExecutorService, SqlExecutorService>();
    }
}