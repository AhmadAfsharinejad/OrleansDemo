using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;
using StreamProcessing.Rest.Logic;
using Xunit;

namespace StreamProcessing.Tests.Rest.Logic;

public class RestResponseConverterTests
{
    private readonly IRestResponseConverter _sut;

    public RestResponseConverterTests()
    {
        _sut = new RestResponseConverter();
    }

    [Fact]
    public async Task Convert_ShouldReturnExpectingRecord_WhenNothingSet()
    {
        //Arrange
        var response = GetHttpResponseMessage();
        var config = new RestConfig();

        var expected = new Dictionary<string, object>();

        //Act
        var actual = await _sut.Convert(response, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Convert_ShouldReturnExpectingRecord_WhenResponseContentFieldNameIsSet()
    {
        //Arrange
        var response = GetHttpResponseMessage();
        var config = new RestConfig
        {
            ResponseContentFieldName = "Content"
        };

        var expected = new Dictionary<string, object> { { config.ResponseContentFieldName, await response.Content.ReadAsStringAsync() } };

        //Act
        var actual = await _sut.Convert(response, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Convert_ShouldReturnExpectingRecord_WhenResponseHeadersIsSet()
    {
        //Arrange
        var response = GetHttpResponseMessage();
        var config = new RestConfig
        {
            ResponseHeaders = new[] { new HeaderField("h1", "f1") }
        };

        var expected = new Dictionary<string, object> { { "f1", "hv1" } };

        //Act
        var actual = await _sut.Convert(response, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public async Task Convert_ShouldReturnExpectingRecord_WhenStatusIsSet()
    {
        //Arrange
        var response = GetHttpResponseMessage();
        var config = new RestConfig
        {
            StatusFieldName = "status"
        };

        var expected = new Dictionary<string, object> { { "status", 202 } };

        //Act
        var actual = await _sut.Convert(response, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Convert_ShouldReturnExpectingRecord_WhenAllAreSet()
    {
        //Arrange
        var response = GetHttpResponseMessage();
        var config = new RestConfig
        {
            ResponseContentFieldName = "Content",
            StatusFieldName = "status",
            ResponseHeaders = new[] { new HeaderField("h1", "f1") },
        };

        var expected = new Dictionary<string, object>
        {
            { config.ResponseContentFieldName, await response.Content.ReadAsStringAsync() },
            { "f1", "hv1" }, { "status", 202 }
        };

        //Act
        var actual = await _sut.Convert(response, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }


    private HttpResponseMessage GetHttpResponseMessage()
    {
        var response = new HttpResponseMessage();
        response.Headers.Add("h1", "hv1");
        response.Headers.Add("h2", "hv2");
        response.StatusCode = (HttpStatusCode)202;
        response.Content = new StringContent("content", Encoding.UTF8);
        return response;
    }
}