using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;
using StreamProcessing.SqlExecutor.Logic;
using Xunit;

namespace StreamProcessing.Tests.SqlExecutor.Logic;

public sealed class SqlExecutorServiceTests
{
    private readonly ISqlExecutorService _sut;
    private readonly IDmlExecutor _dmlExecutor;
    private readonly IDqlReader _dqlReader;
    private readonly IRecordJoiner _recordJoiner;

    public SqlExecutorServiceTests()
    {
        _dmlExecutor = Substitute.For<IDmlExecutor>();
        _dqlReader = Substitute.For<IDqlReader>();
        _recordJoiner = Substitute.For<IRecordJoiner>();
        _sut = new SqlExecutorService(_dmlExecutor, _dqlReader, _recordJoiner);
    }

    [Fact]
    public async Task Execute_ShouldCallDmlExecutor_WhenDmlCommandsIsNotNullOrEmpty()
    {
        // Arrange
        var config = GetSqlExecutorConfig();
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } });

        // Act
        await foreach (var _ in _sut.Execute(default!, default!, config, record, default))
        {
        }

        // Assert
        foreach (var dmlCommand in config.DmlCommands!)
        {
            await _dmlExecutor.Received(1).Execute(Arg.Any<IStreamDbConnection>(), Arg.Any<IStreamDbCommand>(), dmlCommand, record.Record, Arg.Any<CancellationToken>());
        }

        //Assert
        await _dmlExecutor.ReceivedWithAnyArgs(2).Execute(default!, default!, default, default!, default);
    }

    [Theory]
    [InlineData(null)]
    [MemberData(nameof(EmptyDmlCommand))]
    public async Task Execute_ShouldNotCallDmlExecutor_WhenDmlCommandsIsNullOrEmpty(IReadOnlyCollection<DmlCommand>? dmlCommands)
    {
        // Arrange
        var config = GetSqlExecutorConfig();
        config.DmlCommands = dmlCommands;
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } });

        // Act
        await foreach (var _ in _sut.Execute(default!, default!, config, record, default))
        {
        }

        // Assert
        await _dmlExecutor.DidNotReceiveWithAnyArgs().Execute(default!, default!, default, default!, default);
    }

    public static IEnumerable<object[]> EmptyDmlCommand =>
        new List<object[]>
        {
            new object[] { Array.Empty<DmlCommand>() },
        };

    [Fact]
    public async Task Execute_ShouldCallDqlExecutor_WhenDqlCommandIsNotNull()
    {
        // Arrange
        var config = GetSqlExecutorConfig();
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } });

        // Act
        await foreach (var _ in _sut.Execute(default!, default!, config, record, default))
        {
        }

        //Assert
        await foreach (var _ in _dqlReader.Received(1).Read(Arg.Any<IStreamDbConnection>(), Arg.Any<IStreamDbCommand>(), config.DqlCommand!.Value, record.Record, default))
        {
        }
    }

    [Fact]
    public async Task Execute_ShouldNotCallDqlExecutor_WhenDqlCommandIsNull()
    {
        // Arrange
        var config = GetSqlExecutorConfig();
        config.DqlCommand = null;
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } });

        // Act
        await foreach (var _ in _sut.Execute(default!, default!, config, record, default))
        {
        }

        //Assert
        await foreach (var _ in _dqlReader.DidNotReceiveWithAnyArgs().Read(default!, default!, default, default!, default))
        {
        }
    }

    [Fact]
    public async Task Execute_ShouldCallRecordJoiner_WhenDqlCommandReturnValue()
    {
        // Arrange
        var config = GetSqlExecutorConfig();
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } });

        var readResults = new Dictionary<string, object>[]
        {
            new()
            {
                { "f3", 1.3 },
                { "f4", DateTime.Now },
            },
            new()
            {
                { "f3", 2.4 },
                { "f4", DateTime.Now.AddHours(1) },
            }
        };
        _dqlReader.Read(default!, default!, default!, default, default).ReturnsForAnyArgs(readResults.ToAsyncEnumerable());

        // Act
        await foreach (var _ in _sut.Execute(default!, default!, config, record, default))
        {
        }

        //Assert
        foreach (var readResult in readResults)
        {
            _recordJoiner.Received(1).Join(record, readResult, config.JoinType);
        }

        _recordJoiner.ReceivedWithAnyArgs(readResults.Length).Join(default, default!, default);
    }

    [Fact]
    public async Task Execute_ShouldCallRecordJoiner_WhenDqlCommandIsNull()
    {
        // Arrange
        var config = GetSqlExecutorConfig();
        config.DqlCommand = null;
        var record = new PluginRecord(new Dictionary<string, object> { { "f1", 1 }, { "f2", "data" } });

        // Act
        await foreach (var _ in _sut.Execute(default!, default!, config, record, default))
        {
        }

        //Assert
        _recordJoiner.Received(1).Join(record, null, config.JoinType);
    }

    private static SqlExecutorConfig GetSqlExecutorConfig()
    {
        return new SqlExecutorConfig
        {
            ConnectionString = "ConnectionString",
            DmlCommands = GetDmlCommands(),
            DqlCommand = GetDqlCommand(),
            JoinType = RecordJoinType.InputOnly
        };
    }

    private static DqlCommand GetDqlCommand()
    {
        return new DqlCommand
        {
            CommandText = "DqlCommand",
            ParameterFields = new[] { "f2" },
            OutputFields = new[]
            {
                new DqlField("c1", new StreamField("f3", FieldType.Float)),
                new DqlField("c2", new StreamField("f4", FieldType.DateTime))
            }
        };
    }

    private static DmlCommand[] GetDmlCommands()
    {
        return new[]
        {
            new DmlCommand
            {
                CommandText = "DmlCommand1",
                ParameterFields = new[] { "f1", "f2" }
            },
            new DmlCommand
            {
                CommandText = "DmlCommand2",
                ParameterFields = new[] { "f1" }
            }
        };
    }
}