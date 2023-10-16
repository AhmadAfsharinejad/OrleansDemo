using System.Collections.Generic;
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
    private readonly ICommandFiller _commandFiller;

    public DmlExecutorTests()
    {
        _commandFiller = Substitute.For<ICommandFiller>();
        _sut = new DmlExecutor(_commandFiller);
    }

    [Fact]
    public async Task Create_ShouldCallCommandCreator_WhenAll()
    {
        // Arrange
        await using var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dmlCommand = GetDmlCommand();
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        await _sut.Execute(connection, command, dmlCommand, record, default);

        // Assert
        _commandFiller.Received(1).Fill(connection, command, dmlCommand.CommandText, dmlCommand.ParameterFileds, record);
    }

    [Fact]
    public async Task Create_ShouldCallExecuteCommand_WhenAll()
    {
        // Arrange
        await using var connection = new StreamDbConnection(new OdbcConnection());
        var command = Substitute.For<IStreamDbCommand>();
        var dmlCommand = GetDmlCommand();
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        await _sut.Execute(connection, command, dmlCommand, record, default);

        // Assert
        command.Received(1).ExecuteNonQuery();
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