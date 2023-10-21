using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions;
using NSubstitute;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;
using StreamProcessing.Rest.Logic;
using Xunit;

namespace StreamProcessing.Tests.Rest.Logic;

public class RestRequestCreatorTests
{
    private readonly IRestRequestCreator _sut;
    private readonly IStringReplacer _stringReplacer;
    private readonly IUriReplacer _uriReplacer;

    public RestRequestCreatorTests()
    {
        _stringReplacer = Substitute.For<IStringReplacer>();
        _uriReplacer = Substitute.For<IUriReplacer>();
        _sut = new RestRequestCreator(_stringReplacer, _uriReplacer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldDontSetContent_WhenContentIsNullOrEmpty(string? content)
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Get,
            Content = "Content",
            ContentFields = new[] { "f1" }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        _stringReplacer.Replace(config.Content, config.ContentFields, record).Returns(content);

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Content.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldBeenSetContent_WhenContentIsNoNullOrEmpty()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Get,
            Content = "Content",
            ContentFields = new[] { "f1" }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        _stringReplacer.Replace(config.Content, config.ContentFields, record).Returns("xxx");

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Content.Should().BeOfType<StringContent>();
    }

    [Fact]
    public void Create_ShouldHeaderBeEmpty_WhenHeadersAreNotSet()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Get
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Headers.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldHeaderBeEmpty_WhenRequestHeadersIsSet()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Get,
            RequestHeaders = new[]
            {
                new HeaderField("h1", "f1"),
                new HeaderField("h2", "f2")
            }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = new Dictionary<string, List<string>>
        {
            { "h1", new List<string> { "1" } },
            { "h2", new List<string> { "2" } }
        };

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Headers.Count().Should().Be(expected.Keys.Count);

        foreach (var header in actual.Headers)
        {
            var items = expected[header.Key];
            header.Value.Should().BeEquivalentTo(items);
        }
    }

    [Fact]
    public void Create_ShouldHeaderBeEmpty_WhenStaticRequestHeadersIsSet()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Get,
            RequestStaticHeaders = new[]
            {
                new KeyValuePair<string, string>("h3","33"),
                new KeyValuePair<string, string>("h4","44")
            }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = new Dictionary<string, List<string>>
        {
            { "h3", new List<string> { "33" } },
            { "h4", new List<string> { "44" } }
        };

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Headers.Count().Should().Be(expected.Keys.Count);

        foreach (var header in actual.Headers)
        {
            var items = expected[header.Key];
            header.Value.Should().BeEquivalentTo(items);
        }
    }
    
    [Fact]
    public void Create_ShouldHeaderBeEmpty_WhenStaticRequestHeaderAndRequestHeaderAreSet()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Get,
            RequestHeaders = new[]
            {
                new HeaderField("h1", "f1"),
                new HeaderField("h2", "f2")
            },
            RequestStaticHeaders = new[]
            {
                new KeyValuePair<string, string>("h3","33"),
                new KeyValuePair<string, string>("h4","44")
            }
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        var expected = new Dictionary<string, List<string>>
        {
            { "h1", new List<string> { "1" } },
            { "h2", new List<string> { "2" } },
            { "h3", new List<string> { "33" } },
            { "h4", new List<string> { "44" } }
        };

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Headers.Count().Should().Be(expected.Keys.Count);

        foreach (var header in actual.Headers)
        {
            var items = expected[header.Key];
            header.Value.Should().BeEquivalentTo(items);
        }
    }
    
    [Fact]
    public void Create_ShouldMethodBeSet_WhenAll()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Post
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.Method.Should().Be(config.HttpMethod);
    }
    
    [Fact]
    public void Create_ShouldUriBeSet_WhenAll()
    {
        //Arrange
        var config = new RestConfig
        {
            Uri = "Uri",
            HttpMethod = HttpMethod.Post
        };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 2 } });

        _uriReplacer.GetUri(config, record).Returns(config.Uri);
        
        //Act
        var actual = _sut.Create(config, record, default);

        //Assert
        actual.RequestUri!.ToString().Should().Be(config.Uri);
    }
}