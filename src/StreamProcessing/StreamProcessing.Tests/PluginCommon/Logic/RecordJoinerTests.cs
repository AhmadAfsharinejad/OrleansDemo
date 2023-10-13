using System;
using System.Collections.Generic;
using FluentAssertions;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Logic;
using Xunit;

namespace StreamProcessing.Tests.PluginCommon.Logic;

public sealed class RecordJoinerTests
{
    private readonly IRecordJoiner _sut;

    public RecordJoinerTests()
    {
        _sut = new RecordJoiner();
    }

    [Fact]
    public void Join_ShouldReturnInput_WhenJoinTypeIsInputOnly()
    {
        // Arrange
        var input = GetRecord();
        var computeResult = GetComputeResult();

        // Act
        var actual = _sut.Join(input, computeResult, RecordJoinType.InputOnly);

        // Assert
        actual.Should().Be(input);
        actual.Record.Should().Equal(input.Record);
    }

    [Fact]
    public void Join_ShouldTrowException_WhenJoinTypeIsInputOnlyAndInputIsNull()
    {
        // Arrange
        var computeResult = GetComputeResult();

        // Act
        var act = () => _sut.Join(null, computeResult, RecordJoinType.InputOnly);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Join_ShouldReturnComputeResult_WhenJoinTypeIsResultOnly()
    {
        // Arrange
        var input = GetRecord();
        var computeResult = GetComputeResult();

        // Act
        var actual = _sut.Join(input, computeResult, RecordJoinType.ResultOnly);

        // Assert
        actual.Record.Should().Equal(computeResult);
    }

    [Fact]
    public void Join_ShouldThrowException_WhenJoinTypeIsResultOnlyAndComputeIsNull()
    {
        // Arrange
        var input = GetRecord();

        // Act
        var act = () => _sut.Join(input, null, RecordJoinType.ResultOnly);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Join_ShouldReturnAppendedData_WhenJoinTypeIsAppend()
    {
        // Arrange
        var input = GetRecord();
        var computeResult = GetComputeResult();

        var expected = new Dictionary<string, object> { { "i1", "v1" }, { "i2", "v2" }, { "c1", "v1" }, { "c2", "v2" } };

        // Act
        var actual = _sut.Join(input, computeResult, RecordJoinType.Append);

        // Assert
        actual.Record.Should().Equal(expected);
    }

    [Fact]
    public void Join_ShouldReturnInputData_WhenJoinTypeIsAppendAndComputeIsNull()
    {
        // Arrange
        var input = GetRecord();

        // Act
        var actual = _sut.Join(input, null, RecordJoinType.Append);

        // Assert
        actual.Record.Should().Equal(input.Record);
    }

    [Fact]
    public void Join_ShouldReturnComputeData_WhenJoinTypeIsAppendAndInputIsNull()
    {
        // Arrange
        var computeResult = GetComputeResult();

        // Act
        var actual = _sut.Join(null, computeResult, RecordJoinType.Append);

        // Assert
        actual.Record.Should().Equal(computeResult);
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
        var input = GetRecord();
        var computeResult = new Dictionary<string, object> { { "c1", "v1" }, { "i1", "v2" } };

        // Act
        var act = () => _sut.Join(input, computeResult, RecordJoinType.Append);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    private static PluginRecord GetRecord()
    {
        return new PluginRecord
        {
            Record = new Dictionary<string, object> { { "i1", "v1" }, { "i2", "v2" } }
        };
    }
    
    private static Dictionary<string, object> GetComputeResult()
    {
        return new Dictionary<string, object> { { "c1", "v1" }, { "c2", "v2" } };
    }
}