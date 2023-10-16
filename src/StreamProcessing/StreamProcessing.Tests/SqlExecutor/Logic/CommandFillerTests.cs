using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using StreamProcessing.SqlExecutor.Interfaces;
using StreamProcessing.SqlExecutor.Logic;
using Xunit;
// ReSharper disable AccessToDisposedClosure

namespace StreamProcessing.Tests.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed class CommandFillerTests
{
    private readonly ICommandFiller _sut;

    public CommandFillerTests()
    {
        _sut = new CommandFiller();
    }
    
    [Fact]
    public void Fill_ShouldCommentTextBeSameAsInput_WhenAll()
    {
        // Arrange
        using var connection = new StreamDbConnection(new OdbcConnection());
        using var command = connection.CreateStreamDbCommand();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };
        
        // Act
        _sut.Fill(connection, command, commandText, parameters, record);
        
        // Assert
        command.CommandText.Should().Be(commandText);
    }
    
    [Fact]
    public void Fill_ShouldParameterBeSameAsInput_WhenInputParameterIsNotNull()
    {
        // Arrange
        using var connection = new StreamDbConnection(new OdbcConnection());
        using var command = connection.CreateStreamDbCommand();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        _sut.Fill(connection, command, commandText, parameters, record);
        
        // Assert
        command.CommandText.Should().Be(commandText);
        command.Parameters.Count.Should().Be(parameters.Count);
        var enumerator = command.Parameters.GetEnumerator();
        foreach (var parameter in parameters)
        {
            enumerator.MoveNext();
            // ReSharper disable once AssignNullToNotNullAttribute
            ((OdbcParameter)enumerator.Current).Value.Should().Be(record[parameter]);
        }
    }
    
    [Fact]
    public void Fill_ShouldParameterBeEmpty_WhenInputParameterIsNull()
    {
        // Arrange
        using var connection = new StreamDbConnection(new OdbcConnection());
        using var command = connection.CreateStreamDbCommand();
        const string commandText = "command";
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        _sut.Fill(connection, command, commandText, null, record);
        
        // Assert
        command.CommandText.Should().Be(commandText);
        command.Parameters.Count.Should().Be(0);
    }
    
    [Fact]
    public void Fill_ShouldThrowException_WhenInputParameterIsNotInRecord()
    {
        // Arrange
        using var connection = new StreamDbConnection(new OdbcConnection());
        using var command = connection.CreateStreamDbCommand();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c3", "data" } };

        // Act
        var act = () => _sut.Fill(connection, command, commandText, parameters, record);
        
        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }
    
    [Fact]
    public void Fill_ShouldThrowException_WhenInputParameterIsNotEmptyOrNullAndRecordIsNull()
    {
        // Arrange
        using var connection = new StreamDbConnection(new OdbcConnection());
        using var command = connection.CreateStreamDbCommand();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };

        // Act
        var act = () => _sut.Fill(connection, command, commandText, parameters, null);
        
        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}