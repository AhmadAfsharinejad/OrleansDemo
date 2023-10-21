using System.Collections.Generic;
using FluentAssertions;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;
using StreamProcessing.Rest.Logic;
using Xunit;

namespace StreamProcessing.Tests.Rest.Logic;

public class UriReplacerTests
{
    private readonly IUriReplacer _sut;

    public UriReplacerTests()
    {
        _sut = new UriReplacer();
    }

    [Fact]
    public void GetUri_ShouldReturnExpectingUri_WhenOnlyUriIsSet()
    {
        //Arrange
        var uri = "http://domain.com:5400"; 
        var config = new RestConfig
        {
            Uri = uri
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = $"{uri}/";

        //Act
       var actual = _sut.GetUri(config, record);

        //Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void GetUri_ShouldReturnExpectingUri_WhenQueryStringsIsNotNull()
    {
        //Arrange
        var uri = "http://domain.com:5400"; 
        var config = new RestConfig
        {
            Uri = uri,
            QueryStrings = new[] { new QueryStringField("q1", "f1") },
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = $"{uri}/?q1=1";

        //Act
        var actual =_sut.GetUri(config, record);

        //Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void GetUri_ShouldReturnExpectingUri_WhenStaticQueryStringsIsNotNull()
    {
        //Arrange
        var uri = "http://domain.com:5400"; 
        var config = new RestConfig
        {
            Uri = uri,
            StaticQueryStrings = new[] { new KeyValuePair<string, string>("sq1", "1") }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = $"{uri}/?sq1=1";

        //Act
        var actual = _sut.GetUri(config, record);

        //Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void GetUri_ShouldReturnExpectingUri_WhenQueryStringsAndStaticQueryStringsAreNotNull()
    {
        //Arrange
        var uri = "http://domain.com:5400"; 
        var config = new RestConfig
        {
            Uri = uri,
            QueryStrings = new[] { new QueryStringField("q1", "f1") },
            StaticQueryStrings = new[] { new KeyValuePair<string, string>("sq1", "1") }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 11 }, { "f2", 22 } });

        var expected = $"{uri}/?q1=11&sq1=1";

        //Act
       var actual = _sut.GetUri(config, record);

        //Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void GetUri_ShouldReturnExpectingUri_WhenQueryStringsHasDuplicatedKey()
    {
        //Arrange
        var uri = "http://domain.com:5400"; 
        var config = new RestConfig
        {
            Uri = uri,
            StaticQueryStrings = new[]
            {
                new KeyValuePair<string, string>("sq1", "1"),
                new KeyValuePair<string, string>("sq1", "2")
            }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = $"{uri}/?sq1=2";

        //Act
        var actual =_sut.GetUri(config, record);

        //Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void GetUri_ShouldThrowException_WhenFieldIsNotExist()
    {
        //Arrange
        var uri = "http://domain.com:5400"; 
        var config = new RestConfig
        {
            Uri = uri,
            QueryStrings = new[] { new QueryStringField("q1", "f3") },
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        //Act
        var act = () => _sut.GetUri(config, record);

        //Assert
        act.Should().Throw<KeyNotFoundException>();
    }
}