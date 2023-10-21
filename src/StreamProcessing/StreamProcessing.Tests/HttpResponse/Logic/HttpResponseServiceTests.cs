using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NSubstitute;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.HttpResponse.Interfaces;
using StreamProcessing.HttpResponse.Logic;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using Xunit;

namespace StreamProcessing.Tests.HttpResponse.Logic;

public class HttpResponseServiceTests
{
    private readonly IHttpResponseService _sut;
    private readonly IStringReplacer _stringReplacer;

    public HttpResponseServiceTests()
    {
        _stringReplacer = Substitute.For<IStringReplacer>();
        _sut = new HttpResponseService(_stringReplacer);
    }

    [Fact]
    public void GetResponse_ShouldReturnCorrectHeaders_WhenStaticHeadersIsNotNull()
    {
        //Arrange
        var config = new HttpResponseConfig
        {
            StaticHeaders = new[] { new KeyValuePair<string, string>("a", "av1") }
        };

        var record = new PluginRecord(new Dictionary<string, object> { { "f1", "fv1" } });

        var expected = new Dictionary<string, object> { { "a", "av1" } };

        //Act
        var actual = _sut.GetResponse(config, record);

        //Assert
        actual.Headers.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetResponse_ShouldReturnCorrectHeaders_WhenHeadersIsNotNull()
    {
        //Arrange
        var config = new HttpResponseConfig
        {
            Headers = new[] { new HeaderField("h", "f1") }
        };

        var record = new PluginRecord(new Dictionary<string, object> { { "f1", "fv1" } });

        var expected = new Dictionary<string, object> { { "h", "fv1" } };

        //Act
        var actual = _sut.GetResponse(config, record);

        //Assert
        actual.Headers.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetResponse_ShouldReturnCorrectHeaders_WhenHeadersAndStaticHeadersAreNotNull()
    {
        //Arrange
        var config = new HttpResponseConfig
        {
            StaticHeaders = new[] { new KeyValuePair<string, string>("a", "av1") },
            Headers = new[] { new HeaderField("h", "f1") }
        };

        var record = new PluginRecord(new Dictionary<string, object> { { "f1", "fv1" } });

        var expected = new Dictionary<string, object> { { "a", "av1" }, { "h", "fv1" } };

        //Act
        var actual = _sut.GetResponse(config, record);

        //Assert
        actual.Headers.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetResponse_ShouldReturnCorrectContentBytes_WhenContentIsNotNull()
    {
        //Arrange
        var config = new HttpResponseConfig
        {
            Content = "Content",
            ContentFields = new []{"f1"}
        };

        var record = new PluginRecord(new Dictionary<string, object> { { "f1", "fv1" } });

        _stringReplacer.Replace(config.Content, config.ContentFields, record).Returns("result");

        var expected = Encoding.UTF8.GetBytes("result");

        //Act
        var actual = _sut.GetResponse(config, record);

        //Assert
        actual.ContentBytes.Should().BeEquivalentTo(expected);
    }
}