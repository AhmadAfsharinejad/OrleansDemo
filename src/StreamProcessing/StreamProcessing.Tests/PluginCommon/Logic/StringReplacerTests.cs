using System;
using System.Collections.Generic;
using FluentAssertions;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.PluginCommon.Logic;
using Xunit;

namespace StreamProcessing.Tests.PluginCommon.Logic;

public class StringReplacerTests
{
    private readonly IStringReplacer _sut;

    public StringReplacerTests()
    {
        _sut = new StringReplacer();
    }

    [Fact]
    public void Replace_ShouldReturnNull_WhenTemplateIsNull()
    {
        //Arrange
        var contentFields = new List<string> { "f1" };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 1 } });

        //Act
        var actual = _sut.Replace(null, contentFields, record);

        //Assert
        actual.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [MemberData(nameof(EmptyField))]
    public void Replace_ShouldReturnNull_WhenContentFieldsIsNull(List<string>? fields)
    {
        //Arrange
        var contentTemplate = "Template";
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 1 } });

        //Act
        var actual = _sut.Replace(contentTemplate, fields, record);

        //Assert
        actual.Should().Be(contentTemplate);
    }

    public static IEnumerable<object[]> EmptyField =>
        new List<object[]> { new object[] { new List<string>() } };

    [Fact]
    public void Replace_ShouldReturnReplaceValue_WhenTemplateAndContentFieldsAreNotNull()
    {
        //Arrange
        var contentTemplate = "Template {0}";
        var contentFields = new List<string> { "f1" };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 1 } });

        var expected = "Template 1";
        
        //Act
        var actual = _sut.Replace(contentTemplate, contentFields, record);

        //Assert
        actual.Should().Be(expected);
    }
    
    [Fact]
    public void Replace_ShouldThrowException_WhenTemplateContentFieldIsNotExist()
    {
        //Arrange
        var contentTemplate = "Template {0}";
        var contentFields = new List<string> { "ff1" };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 1 } });

        //Act
        var act = () => _sut.Replace(contentTemplate, contentFields, record);

        //Assert
        act.Should().Throw<KeyNotFoundException>();
    }
    
    [Fact]
    public void Replace_ShouldThrowException_WhenTemplateHasMoreFieldThanContentFields()
    {
        //Arrange
        var contentTemplate = "Template {0} {1}";
        var contentFields = new List<string> { "f1" };
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", 1 } });

        //Act
        var act = () => _sut.Replace(contentTemplate, contentFields, record);

        //Assert
        act.Should().Throw<FormatException>();
    }
}