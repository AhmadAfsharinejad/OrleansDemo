using NSubstitute;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;
using Xunit;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace StreamProcessing.Tests.Rest;

public class RestGrainTests
{
    private readonly IRestGrain _sut;
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<RestConfig> _pluginConfigFetcher;
    private readonly IRestService _restService;
    private readonly IRestOutputFieldTypeGetter _restOutputFieldTypeGetter;
    
    public RestGrainTests()
    {
        _pluginOutputCaller = Substitute.For<IPluginOutputCaller>();
        _pluginConfigFetcher = Substitute.For<IPluginConfigFetcher<RestConfig> >();
        _restService = Substitute.For<IRestService>();
        _restOutputFieldTypeGetter = Substitute.For<IRestOutputFieldTypeGetter>();
        _sut = new RestGrain(_pluginOutputCaller, _pluginConfigFetcher, _restService, _restOutputFieldTypeGetter);
    }

    [Fact]
    public void Compute_ShouldCallRestOutputFieldTypeGetterOneTime_WhenCallMultipleTimes()
    {
        //Arrange

        //Act
        _sut.Compute(default, new PluginRecord(), default!);
        _sut.Compute(default, new PluginRecords(), default!);

        //Assert
        _restOutputFieldTypeGetter.ReceivedWithAnyArgs(1).GetOutputs(default, default);
    }
}