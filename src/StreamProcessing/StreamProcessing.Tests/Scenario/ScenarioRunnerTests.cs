using System;
using NSubstitute;
using Orleans;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Scenario;
using StreamProcessing.Scenario.Domain;
using StreamProcessing.Scenario.Interfaces;
using StreamProcessing.Tests.PluginCommon.Logic.Mock;
using Xunit;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace StreamProcessing.Tests.Scenario;

public class ScenarioRunnerTests
{
    private readonly IScenarioRunner _sut;
    private readonly IGrainFactory _grainFactory;
    private readonly IPluginGrainFactory _pluginGrainFactory;

    public ScenarioRunnerTests()
    {
        _grainFactory = Substitute.For<IGrainFactory>();
        _pluginGrainFactory = Substitute.For<IPluginGrainFactory>();
        _sut = new ScenarioRunner(_grainFactory, _pluginGrainFactory);
    }

    [Fact]
    public void Run_ShouldCallStart_WhenPluginIsSource()
    {
        //Arrange
        var source1 = new PluginConfig(new PluginTypeId("source"), Guid.NewGuid(), new MockPluginConfig());
        var source2 = new PluginConfig(new PluginTypeId("source2"), Guid.NewGuid(), new MockPluginConfig());
        var middle = new PluginConfig(new PluginTypeId("middle"), Guid.NewGuid(), new MockPluginConfig());
        var target = new PluginConfig(new PluginTypeId("target"), Guid.NewGuid(), new MockPluginConfig());

        var config = new ScenarioConfig
        {
            Id = Guid.NewGuid(),
            Configs = new[] { source1, source2, middle, target },
            Relations = new[]
            {
                new LinkConfig(source1.Id, middle.Id),
                new LinkConfig(middle.Id, target.Id),
                new LinkConfig(source2.Id, target.Id)
            }
        };

        var sourcePlugin1 = Substitute.For<ISourcePluginGrain>();
        _pluginGrainFactory.GetOrCreateSourcePlugin(source1.PluginTypeId, source1.Id).Returns(sourcePlugin1);

        var sourcePlugin2 = Substitute.For<ISourcePluginGrain>();
        _pluginGrainFactory.GetOrCreateSourcePlugin(source2.PluginTypeId, source2.Id).Returns(sourcePlugin2);
        
        //Act
        _sut.Run(config);

        //Assert
        _pluginGrainFactory.ReceivedWithAnyArgs(2).GetOrCreateSourcePlugin(new PluginTypeId(), Guid.Empty);
        sourcePlugin1.Received(1).Start(new PluginExecutionContext(config.Id, source1.Id, null), Arg.Any<GrainCancellationToken>());
        sourcePlugin2.Received(1).Start(new PluginExecutionContext(config.Id, source2.Id, null), Arg.Any<GrainCancellationToken>());
    }
}