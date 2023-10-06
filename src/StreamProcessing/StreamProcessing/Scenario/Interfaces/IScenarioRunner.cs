using StreamProcessing.Scenario.Domain;

namespace StreamProcessing.Scenario.Interfaces;

public interface IScenarioRunner
{
    Task Run(ScenarioConfig config);
}