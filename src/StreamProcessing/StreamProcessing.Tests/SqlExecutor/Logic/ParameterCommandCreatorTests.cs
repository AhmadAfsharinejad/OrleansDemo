using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using StreamProcessing.SqlExecutor.Interfaces;
using StreamProcessing.SqlExecutor.Logic;
using Xunit;

namespace StreamProcessing.Tests.SqlExecutor.Logic;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed class ParameterCommandCreatorTests
{
    private readonly IParameterCommandCreator _sut;

    public ParameterCommandCreatorTests()
    {
        _sut = new ParameterCommandCreator();
    }
    
    [Fact]
    public void Create_ShouldCreateCommandFromInputConnection_WhenAll()
    {
        // Arrange
        var connection = new OdbcConnection();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };
        
        // Act
        var actual = _sut.Create(connection, commandText, parameters, record);
        
        // Assert
        actual.Connection.Should().Be(connection);
    }

    [Fact]
    public void Create_ShouldCommentTextBeSameAsInput_WhenAll()
    {
        // Arrange
        var connection = new OdbcConnection();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };
        
        // Act
        var actual = _sut.Create(connection, commandText, parameters, record);
        
        // Assert
        actual.CommandText.Should().Be(commandText);
    }
    
    [Fact]
    public void Create_ShouldParameterBeSameAsInput_WhenInputParameterIsNotNull()
    {
        // Arrange
        var connection = new OdbcConnection();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        var actual = _sut.Create(connection, commandText, parameters, record);
        
        // Assert
        actual.CommandText.Should().Be(commandText);
        actual.Parameters.Count.Should().Be(parameters.Count);
        var enumerator = actual.Parameters.GetEnumerator();
        foreach (var parameter in parameters)
        {
            enumerator.MoveNext();
            // ReSharper disable once AssignNullToNotNullAttribute
            ((OdbcParameter)enumerator.Current).Value.Should().Be(record[parameter]);
        }
    }
    
    [Fact]
    public void Create_ShouldParameterBeEmpty_WhenInputParameterIsNull()
    {
        // Arrange
        var connection = new OdbcConnection();
        const string commandText = "command";
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c2", "data" } };

        // Act
        var actual = _sut.Create(connection, commandText, null, record);
        
        // Assert
        actual.CommandText.Should().Be(commandText);
        actual.Parameters.Count.Should().Be(0);
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenInputParameterIsNotInRecord()
    {
        // Arrange
        var connection = new OdbcConnection();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };
        var record = new Dictionary<string, object> { { "c1", 1 }, { "c3", "data" } };

        // Act
        var act = () => _sut.Create(connection, commandText, parameters, record);
        
        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_WhenInputParameterIsNotEmptyOrNullAndRecordIsNull()
    {
        // Arrange
        var connection = new OdbcConnection();
        const string commandText = "command";
        var parameters = new List<string> { "c1", "c2" };

        // Act
        var act = () => _sut.Create(connection, commandText, parameters, null);
        
        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}