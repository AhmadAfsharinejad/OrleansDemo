using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NSubstitute;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;
using StreamProcessing.SqlExecutor.Logic;
using Xunit;

namespace StreamProcessing.Tests.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed class DmlExecutorTests
{
    private readonly IDmlExecutor _sut;
    private readonly IParameterCommandCreator _commandCreator;

    public DmlExecutorTests()
    {
        _commandCreator = Substitute.For<IParameterCommandCreator>();
        _sut = new DmlExecutor(_commandCreator);
    }

    [Fact]
    public async Task Create_ShouldCallCommandCreator_WhenAll()
    {
        // Arrange
        var connection = new OdbcConnection();
        var command = GetDmlCommand();
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        await _sut.Execute(connection, command, record);

        // Assert
        _commandCreator.Received(1).Create(connection, command.CommandText, command.ParameterFileds, record);
    }

    [Fact]
    public async Task Create_ShouldCallExecuteCommand_WhenAll()
    {
        // Arrange
        var connection = new OdbcConnection();
        var dmlCommand = GetDmlCommand();
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        var command = Substitute.For<IDbCommand>();
        _commandCreator.Create(default!, default!, default, default).ReturnsForAnyArgs(command);

        // Act
        await _sut.Execute(connection, dmlCommand, record);

        // Assert
        command.Received(1).ExecuteNonQuery();
        command.Received(1).Dispose();
    }

    private static DmlCommand GetDmlCommand()
    {
        return new DmlCommand
        {
            CommandText = "command",
            ParameterFileds = new[] { "c1", "c2" }
        };
    }
}