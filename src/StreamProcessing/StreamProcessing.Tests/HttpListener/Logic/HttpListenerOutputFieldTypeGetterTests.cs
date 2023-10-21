using System.Collections.Generic;
using FluentAssertions;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.HttpListener.Logic;
using StreamProcessing.PluginCommon.Domain;
using Xunit;

namespace StreamProcessing.Tests.HttpListener.Logic;

public class HttpListenerOutputFieldTypeGetterTests
{
    private readonly IHttpListenerOutputFieldTypeGetter _sut;

    public HttpListenerOutputFieldTypeGetterTests()
    {
        _sut = new HttpListenerOutputFieldTypeGetter();
    }

    [Fact]
    public void GetOutputs_ShouldReturnHeadersAsOutput_WhenHeadersIsNotNull()
    {
        //Arrange
        var config = new HttpListenerConfig
        {
            Headers = new[]
            {
                new HeaderField("a", "aa"),
                new HeaderField("b", "bb")
            }
        };

        var expected = new Dictionary<string, FieldType>
        {
            { "aa", FieldType.Text }, { "bb", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetOutputs_ShouldReturnQueryStringsAsOutput_WhenQueryStringsIsNotNull()
    {
        //Arrange
        var config = new HttpListenerConfig
        {
            QueryStrings = new[]
            {
                new QueryStringField("a", "aa"),
                new QueryStringField("b", "bb")
            }
        };

        var expected = new Dictionary<string, FieldType>
        {
            { "aa", FieldType.Text }, { "bb", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetOutputs_ShouldReturnContentFieldNameAsOutput_WhenQueryStringsIsNotNull()
    {
        //Arrange
        var config = new HttpListenerConfig
        {
            ContentFieldName = "cfn"
        };

        var expected = new Dictionary<string, FieldType>
        {
            { "cfn", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetOutputs_ShouldReturnOutput_WhenQueryAllHasValue()
    {
        //Arrange
        var config = new HttpListenerConfig
        {
            Headers = new[]
            {
                new HeaderField("a", "aa"),
                new HeaderField("b", "bb")
            },
            QueryStrings = new[]
            {
                new QueryStringField("c", "cc"),
                new QueryStringField("d", "dd")
            },
            ContentFieldName = "cfn"
        };

        var expected = new Dictionary<string, FieldType>
        {
            { "aa", FieldType.Text }, { "bb", FieldType.Text },
            { "cc", FieldType.Text }, { "dd", FieldType.Text },
            { "cfn", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
}