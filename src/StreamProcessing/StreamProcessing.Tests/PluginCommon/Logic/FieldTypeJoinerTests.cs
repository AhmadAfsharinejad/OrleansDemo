using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.PluginCommon.Logic;
using Xunit;

namespace StreamProcessing.Tests.PluginCommon.Logic;

public sealed class FieldTypeJoinerTests
{
    private readonly IFieldTypeJoiner _sut;

    public FieldTypeJoinerTests()
    {
        _sut = new FieldTypeJoiner();
    }

    [Fact]
    public void Join_ShouldReturnInput_WhenJoinTypeIsInputOnly()
    {
        // Arrange
        var inputTypes = GetInputTypes();
        var pluginFields = GetPluginFields();

        // Act
        var actual = _sut.Join(inputTypes, pluginFields, RecordJoinType.InputOnly);

        // Assert
        actual.Should().Equal(inputTypes);
    }


    [Fact]
    public void Join_ShouldTrowException_WhenJoinTypeIsInputOnlyAndInputIsNull()
    {
        // Arrange
        var pluginFields = GetPluginFields();

        // Act
        var act = () => _sut.Join(null, pluginFields, RecordJoinType.InputOnly);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Join_ShouldReturnComputeResult_WhenJoinTypeIsResultOnly()
    {
        // Arrange
        var inputTypes = GetInputTypes();
        var pluginFields = GetPluginFields();
        var expected = pluginFields.ToDictionary(x => x.Name, y => y.Type);

        // Act
        var actual = _sut.Join(inputTypes, pluginFields, RecordJoinType.ResultOnly);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Join_ShouldThrowException_WhenJoinTypeIsResultOnlyAndComputeIsNull()
    {
        // Arrange
        var inputTypes = GetInputTypes();

        // Act
        var act = () => _sut.Join(inputTypes, null, RecordJoinType.ResultOnly);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Join_ShouldReturnAppendedData_WhenJoinTypeIsAppend()
    {
        // Arrange
        var inputTypes = GetInputTypes();
        var pluginFields = GetPluginFields();

        var expected = new Dictionary<string, FieldType>
        {
            { "i1", FieldType.Guid }, { "i2", FieldType.Date }, { "f1", FieldType.Integer }, { "f2", FieldType.Bool }
        };

        // Act
        var actual = _sut.Join(inputTypes, pluginFields, RecordJoinType.Append);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Join_ShouldReturnInputData_WhenJoinTypeIsAppendAndComputeIsNull()
    {
        // Arrange
        var inputTypes = GetInputTypes();

        // Act
        var actual = _sut.Join(inputTypes, null, RecordJoinType.Append);

        // Assert
        actual.Should().Equal(inputTypes);
    }

    [Fact]
    public void Join_ShouldReturnComputeData_WhenJoinTypeIsAppendAndInputIsNull()
    {
        // Arrange
        var pluginFields = GetPluginFields();
        var expected = pluginFields.ToDictionary(x => x.Name, y => y.Type);

        // Act
        var actual = _sut.Join(null, pluginFields, RecordJoinType.Append);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Join_ShouldThrowException_WhenJoinTypeIsAppendAndBothInputAndComputeAreNull()
    {
        // Arrange

        // Act
        var act = () => _sut.Join(null, null, RecordJoinType.ResultOnly);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Join_ShouldThrowException_WhenJoinTypeIsAppendAndComputeResultHasSameKeyWithInput()
    {
        // Arrange
        var inputTypes = GetInputTypes();
        var pluginFields = new List<StreamField>
        {
            new("f1", FieldType.Integer),
            new("i2", FieldType.Bool)
        };

        // Act
        var act = () => _sut.Join(inputTypes, pluginFields, RecordJoinType.Append);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    private static Dictionary<string, FieldType> GetInputTypes()
    {
        return new Dictionary<string, FieldType> { { "i1", FieldType.Guid }, { "i2", FieldType.Date } };
    }

    private static List<StreamField> GetPluginFields()
    {
        return new List<StreamField>
        {
            new("f1", FieldType.Integer),
            new("f2", FieldType.Bool)
        };
    }
}