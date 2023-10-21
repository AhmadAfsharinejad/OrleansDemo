using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Orleans;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.SqlExecutor;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;
using Xunit;

namespace StreamProcessing.Tests.SqlExecutor;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed class SqlExecutorGrainTests
{
    private readonly ISqlExecutorGrain _sut;
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<SqlExecutorConfig> _pluginConfigFetcher;
    private readonly IConnectionFactory _connectionFactory;
    private readonly ISqlExecutorService _sqlExecutorService;
    private readonly IFieldTypeJoiner _fieldTypeJoiner;

    public SqlExecutorGrainTests()
    {
        _pluginOutputCaller = Substitute.For<IPluginOutputCaller>();
        _pluginConfigFetcher = Substitute.For<IPluginConfigFetcher<SqlExecutorConfig>>();
        _connectionFactory = Substitute.For<IConnectionFactory>();
        _sqlExecutorService = Substitute.For<ISqlExecutorService>();
        _fieldTypeJoiner = Substitute.For<IFieldTypeJoiner>();
        _sut = new SqlExecutorGrain(_pluginOutputCaller, _pluginConfigFetcher, _connectionFactory, _sqlExecutorService, _fieldTypeJoiner);
    }
    
    [Fact]
    public async Task Compute_ShouldCallConnectionFactoryOneTime_WhenCallComputeMultipleTime()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig
        {
            ConnectionString = "ConnectionString"
        };
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);

        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, new PluginRecords(Array.Empty<PluginRecord>()), tcs.Token);
        await _sut.Compute(pluginContext, new PluginRecords(Array.Empty<PluginRecord>()), tcs.Token);

        // Assert
        _connectionFactory.Received(1).Create(pluginConfig.ConnectionString);
        _connectionFactory.ReceivedWithAnyArgs(1).Create(default!);
    }
    
    [Fact]
    public async Task Compute_ShouldCallFieldTypeJoinerOneTime_WhenCallComputeMultipleTime()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig()
        {
            DqlCommand = new DqlCommand
            {
                OutputFields = new []
                {
                    new DqlField("bDb", new StreamField("b", FieldType.Bool)),
                    new DqlField("tDb", new StreamField("t", FieldType.Text)),
                }
            }
        };
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);

        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, new PluginRecords(Array.Empty<PluginRecord>()), tcs.Token);
        await _sut.Compute(pluginContext, new PluginRecords(Array.Empty<PluginRecord>()), tcs.Token);

        // Assert
        _fieldTypeJoiner.Received(1).Join(pluginContext.InputFieldTypes, 
            Arg.Is<IEnumerable<StreamField>>(x => x.SequenceEqual(pluginConfig.DqlCommand!.Value.OutputFields.Select(z => z.Field))),
            pluginConfig.JoinType);
        
        _fieldTypeJoiner.ReceivedWithAnyArgs(1).Join(default, default, default);
    }

    [Fact]
    public async Task Start_ShouldCallSqlExecutorService_WhenAll()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig();
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);

        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Start(pluginContext, tcs.Token);

        // Assert
        await foreach (var _ in _sqlExecutorService.Received(1)
                           .Execute(Arg.Any<IStreamDbConnection>(), Arg.Any<IStreamDbCommand>(),
                               pluginConfig, null,
                               Arg.Any<CancellationToken>()))
        {
        }
    }

    [Fact]
    public async Task Compute_ShouldCallSqlExecutorServicePerRecord_WhenInputIsNotNull()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig();
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);
        var records = GetRecords();

        // Act

        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, records, tcs.Token);

        // Assert
        foreach (var record in records.Records)
        {
            await foreach (var _ in _sqlExecutorService.Received(1)
                               .Execute(Arg.Any<IStreamDbConnection>(),Arg.Any<IStreamDbCommand>(),
                                   pluginConfig, record,
                                   Arg.Any<CancellationToken>()))
            {
            }    
        }
    }
    
    [Fact]
    public async Task Compute_ShouldCallPluginOutputCallerOneTime_WhenInputIsNull()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig();
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);
        var records = GetRecords();
        
        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, records, tcs.Token);

        // Assert
        await _pluginOutputCaller.ReceivedWithAnyArgs(1).CallOutputs(default, new List<PluginRecord>(), default!);
    }

    [Fact]
    public async Task Compute_ShouldCallPluginOutputCallerOneTime_WhenInputIsNotNull()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig();
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);

        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, new PluginRecords(Array.Empty<PluginRecord>()), tcs.Token);

        // Assert
        await _pluginOutputCaller.ReceivedWithAnyArgs(1).CallOutputs(default, new List<PluginRecord>(), default!);
    }
    
    private static PluginRecords GetRecords()
    {
        return new PluginRecords
        {
            Records = new[]
            {
                new PluginRecord(new Dictionary<string, object> { { "f1", "v1" } }),
                new PluginRecord(new Dictionary<string, object> { { "f2", "v2" } })
            }
        };
    }

    private static PluginExecutionContext GetPluginContext()
    {
        return new PluginExecutionContext
        {
            PluginId = Guid.NewGuid(),
            ScenarioId = Guid.NewGuid(),
            InputFieldTypes = new Dictionary<string, FieldType>
            {
                { "f1", FieldType.Date }, { "f2", FieldType.Guid }
            }
        };
    }
}