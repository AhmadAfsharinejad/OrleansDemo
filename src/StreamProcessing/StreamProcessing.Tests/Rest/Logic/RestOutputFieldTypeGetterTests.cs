using System.Collections.Generic;
using System.Linq;
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

public class RestOutputFieldTypeGetterTests
{
    private readonly IRestOutputFieldTypeGetter _sut;
    private readonly IFieldTypeJoiner _fieldTypeJoiner;

    public RestOutputFieldTypeGetterTests()
    {
        _fieldTypeJoiner = Substitute.For<IFieldTypeJoiner>();
        _sut = new RestOutputFieldTypeGetter(_fieldTypeJoiner);
    }

    [Fact]
    public void GetOutputs_ShouldReturnExpected_WhenNothingIsSet()
    {
        //Arrange
        var context = new PluginExecutionContext
        {
            InputFieldTypes = new Dictionary<string, FieldType> { { "f1", FieldType.Integer } }
        };
        var config = new RestConfig();

        _fieldTypeJoiner.Join(context.InputFieldTypes, Arg.Any<IEnumerable<StreamField>?>(), config.JoinType)
            .Returns(args => (args[1] as IEnumerable<StreamField>)!.ToDictionary(x => x.Name, x => x.Type));

        var expected = new Dictionary<string, FieldType>();

        //Act
        var actual = _sut.GetOutputs(context, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetOutputs_ShouldReturnExpected_WhenResponseHeadersIsSet()
    {
        //Arrange
        var context = new PluginExecutionContext
        {
            InputFieldTypes = new Dictionary<string, FieldType> { { "f1", FieldType.Integer } }
        };
        var config = new RestConfig
        {
            ResponseHeaders = new[] { new HeaderField("h1", "f1") }
        };

        _fieldTypeJoiner.Join(context.InputFieldTypes, Arg.Any<IEnumerable<StreamField>?>(), config.JoinType)
            .Returns(args => (args[1] as IEnumerable<StreamField>)!.ToDictionary(x => x.Name, x => x.Type));

        var expected = new Dictionary<string, FieldType>
        {
            { "f1", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(context, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetOutputs_ShouldReturnExpected_WhenResponseContentFieldNameIsSet()
    {
        //Arrange
        var context = new PluginExecutionContext
        {
            InputFieldTypes = new Dictionary<string, FieldType> { { "f1", FieldType.Integer } }
        };
        var config = new RestConfig
        {
            ResponseContentFieldName = "Content"
        };

        _fieldTypeJoiner.Join(context.InputFieldTypes, Arg.Any<IEnumerable<StreamField>?>(), config.JoinType)
            .Returns(args => (args[1] as IEnumerable<StreamField>)!.ToDictionary(x => x.Name, x => x.Type));

        var expected = new Dictionary<string, FieldType>
        {
            { "Content", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(context, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetOutputs_ShouldReturnExpected_WhenStatusFieldNameIsSet()
    {
        //Arrange
        var context = new PluginExecutionContext
        {
            InputFieldTypes = new Dictionary<string, FieldType> { { "f1", FieldType.Integer } }
        };
        var config = new RestConfig
        {
            StatusFieldName = "Status"
        };

        _fieldTypeJoiner.Join(context.InputFieldTypes, Arg.Any<IEnumerable<StreamField>?>(), config.JoinType)
            .Returns(args => (args[1] as IEnumerable<StreamField>)!.ToDictionary(x => x.Name, x => x.Type));

        var expected = new Dictionary<string, FieldType>
        {
            { "Status", FieldType.Integer }
        };

        //Act
        var actual = _sut.GetOutputs(context, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetOutputs_ShouldReturnExpected_WhenAllAreSet()
    {
        //Arrange
        var context = new PluginExecutionContext
        {
            InputFieldTypes = new Dictionary<string, FieldType> { { "f1", FieldType.Integer } }
        };
        var config = new RestConfig
        {
            ResponseContentFieldName = "Content",
            StatusFieldName = "Status",
            ResponseHeaders = new[] { new HeaderField("h1", "f1") }
        };

        _fieldTypeJoiner.Join(context.InputFieldTypes, Arg.Any<IEnumerable<StreamField>?>(), config.JoinType)
            .Returns(args => (args[1] as IEnumerable<StreamField>)!.ToDictionary(x => x.Name, x => x.Type));

        var expected = new Dictionary<string, FieldType>
        {
            { "Status", FieldType.Integer },
            { "Content", FieldType.Text },
            { "f1", FieldType.Text }
        };

        //Act
        var actual = _sut.GetOutputs(context, config);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }
}