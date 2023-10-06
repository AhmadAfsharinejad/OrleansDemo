using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.Scenario.Interfaces;

namespace StreamProcessing.Scenario;

internal sealed class ScenarioDiIntroducer : IDependencyIntroducer
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IScenarioRunner, ScenarioRunner>();
    }
}