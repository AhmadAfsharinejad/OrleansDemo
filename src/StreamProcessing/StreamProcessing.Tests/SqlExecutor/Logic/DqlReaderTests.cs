using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;
using StreamProcessing.SqlExecutor.Logic;
using Xunit;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace StreamProcessing.Tests.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
[SuppressMessage("Performance", "CA1806:Do not ignore method results")]
public sealed class DqlReaderTests
{
    private readonly IDqlReader _sut;
    private readonly ICommandFiller _commandFiller;

    public DqlReaderTests()
    {
        _commandFiller = Substitute.For<ICommandFiller>();
        _sut = new DqlReader(_commandFiller);
    }

    [Fact]
    public async Task Create_ShouldCallCommandCreator_WhenAll()
    {
        // Arrange
        var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dqlCommand = GetDqlCommand();
        var record = new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } };

        // Act
        await foreach (var _ in _sut.Read(connection, command, dqlCommand, record, default))
        {
        }

        // Assert
        _commandFiller.Received(1).Fill(connection, command, dqlCommand.CommandText, dqlCommand.ParameterFileds, record);
    }

    [Fact]
    public async Task Create_ShouldCallExecuteReader_WhenAll()
    {
        // Arrange
        var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dqlCommand = GetDqlCommand();
        var record = new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } };

        var reader = Substitute.For<IDataReader>();
        command.ExecuteReader().Returns(reader);

        // Act
        await foreach (var _ in _sut.Read(connection, command, dqlCommand, record, default))
        {
        }

        // Assert
        command.Received(1).ExecuteReader();
    }

    [Fact]
    public async Task Create_ShouldReadResult_WhenDataReaderHasValue()
    {
        // Arrange
        var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dqlCommand = GetDqlCommand();
        var record = new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } };

        var reader = Substitute.For<IDataReader>();
        command.ExecuteReader().Returns(reader);

        reader.Read().Returns(true, false);

        // Act
        await foreach (var _ in _sut.Read(connection, command, dqlCommand, record, default))
        {
        }

        // Assert
        reader.Received(2).Read();
        reader.Received(1).Dispose();
    }

    [Fact]
    public async Task Create_ShouldReturnResult_WhenRead()
    {
        // Arrange
        var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dqlCommand = GetDqlCommand();
        var record = new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } };

        var reader = Substitute.For<IDataReader>();
        command.ExecuteReader().Returns(reader);

        reader.Read().Returns(true, true, false);

        reader["c1"].Returns(3, 4);
        reader["c2"].Returns("v3", "v4");

        var expecteds = new List<Dictionary<string, object>>
        {
            new() { { "f3", 3 }, { "f4", "v3" } },
            new() { { "f3", 4 }, { "f4", "v4" } }
        };

        // Act
        var actual = _sut.Read(connection, command, dqlCommand, record, default);

        // Assert
        var index = 0;
        await foreach (var result in actual)
        {
            result.Should().BeEquivalentTo(expecteds[index]);
            index++;
        }

        index.Should().Be(expecteds.Count);
    }

    [Fact]
    public async Task Create_ShouldThrowException_WhenDbColumnIsNotExist()
    {
        // Arrange
        var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dqlCommand = new DqlCommand
        {
            CommandText = "command",
            ParameterFileds = new[] { "f1", "f2" },
            OutputFileds = new[]
            {
                new DqlField("c1", new StreamField("f3", FieldType.Integer)),
                new DqlField("c4", new StreamField("f4", FieldType.Text))
            }
        };
        var record = new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } };

        var reader = Substitute.For<IDataReader>();
        command.ExecuteReader().Returns(reader);

        reader.Read().Returns(true, true, false);

        reader[Arg.Any<string>()].Returns(_ => throw new IndexOutOfRangeException());
        reader[Arg.Is("c1")].Returns(3, 4);

        // Act
        var act = async () =>
        {
            await foreach (var _ in _sut.Read(connection, command, dqlCommand, record, default))
            {
            }
        };

        // Assert
        await act.Should().ThrowAsync<IndexOutOfRangeException>();
    }

    private static DqlCommand GetDqlCommand()
    {
        return new DqlCommand
        {
            CommandText = "command",
            ParameterFileds = new[] { "f1", "f2" },
            OutputFileds = new[]
            {
                new DqlField("c1", new StreamField("f3", FieldType.Integer)),
                new DqlField("c2", new StreamField("f4", FieldType.Text))
            }
        };
    }
}