using NSubstitute;
using StreamProcessing.Map;
using StreamProcessing.Map.Domain;
using StreamProcessing.Map.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using Xunit;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace StreamProcessing.Tests.Map;

public class MapGrainTests
{
    private readonly IMapGrain _sut;
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<MapConfig> _pluginConfigFetcher;
    private readonly ICompiler _compiler;
    
    public MapGrainTests()
    {
        _pluginOutputCaller = Substitute.For<IPluginOutputCaller>();
        _pluginConfigFetcher = Substitute.For<IPluginConfigFetcher<MapConfig>>();
        _compiler = Substitute.For<ICompiler>();
        _sut = new MapGrain(_pluginOutputCaller, _pluginConfigFetcher, _compiler);
    }
    
    [Fact]
    public void Compute_ShouldCallCompileCreateFunctionOneTime_WhenCallMultipleTimes()
    {
        //Arrange
        var config = new MapConfig
        {
            OutputColumns = new[] { new StreamField("f1", FieldType.Text) }
        };

        _pluginConfigFetcher.GetConfig(default, default).ReturnsForAnyArgs(config);

        //Act
        _sut.Compute(default, new PluginRecord(), default!);
        _sut.Compute(default, new PluginRecords(), default!);

        //Assert
        _compiler.ReceivedWithAnyArgs(1).CreateFunction(default!, default!, default!);
    }
}