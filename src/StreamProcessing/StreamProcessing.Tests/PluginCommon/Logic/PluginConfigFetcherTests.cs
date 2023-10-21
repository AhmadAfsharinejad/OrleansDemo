using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Orleans;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.PluginCommon.Logic;
using StreamProcessing.Scenario.Interfaces;
using StreamProcessing.Tests.PluginCommon.Logic.Mock;
using Xunit;

namespace StreamProcessing.Tests.PluginCommon.Logic;

public class PluginConfigFetcherTests
{
    private readonly IPluginConfigFetcher<MockPluginConfig> _sut;
    private readonly IGrainFactory _grainFactory;

    public PluginConfigFetcherTests()
    {
        _grainFactory = Substitute.For<IGrainFactory>();
        _sut = new PluginConfigFetcher<MockPluginConfig>(_grainFactory);
    }

    [Fact]
    public void GetConfig_ShouldCallGetGrainOneTime_WhenAll()
    {
        //Arrange
        var scenarioId = Guid.NewGuid();
        var pluginId = Guid.NewGuid();
        
        var scenarioGrain = Substitute.For<IScenarioGrain>();
        _grainFactory.GetGrain<IScenarioGrain>(scenarioId).Returns(scenarioGrain);
        scenarioGrain.GetPluginConfig(pluginId).Returns(new MockPluginConfig());
        
        //Act
        _sut.GetConfig(scenarioId, pluginId);
        _sut.GetConfig(scenarioId, pluginId);

        //Assert
        _grainFactory.Received(1).GetGrain<IScenarioGrain>(scenarioId);
        _grainFactory.ReceivedWithAnyArgs(1).GetGrain<IScenarioGrain>(scenarioId);
    }
    
    [Fact]
    public void GetConfig_ShouldCallGetPluginConfigOneTime_WhenAll()
    {
        //Arrange
        var scenarioId = Guid.NewGuid();
        var pluginId = Guid.NewGuid();

        var scenarioGrain = Substitute.For<IScenarioGrain>();
        _grainFactory.GetGrain<IScenarioGrain>(scenarioId).Returns(scenarioGrain);
        scenarioGrain.GetPluginConfig(pluginId).Returns(new MockPluginConfig());
        
        //Act
        _sut.GetConfig(scenarioId, pluginId);
        _sut.GetConfig(scenarioId, pluginId);

        //Assert
        scenarioGrain.Received(1).GetPluginConfig(pluginId);
        scenarioGrain.ReceivedWithAnyArgs(1).GetPluginConfig(default);
    }
    
    [Fact]
    public async Task GetConfig_ShouldThrowException_WhenConfigTypeIsNotSame()
    {
        //Arrange
        var scenarioId = Guid.NewGuid();
        var pluginId = Guid.NewGuid();

        var scenarioGrain = Substitute.For<IScenarioGrain>();
        _grainFactory.GetGrain<IScenarioGrain>(scenarioId).Returns(scenarioGrain);
        scenarioGrain.GetPluginConfig(pluginId).Returns(new OtherMockPluginConfig());
        
        //Act
        var act = async () => await _sut.GetConfig(scenarioId, pluginId);

        //Assert
        await act.Should().ThrowAsync<InvalidCastException>();
    }
}