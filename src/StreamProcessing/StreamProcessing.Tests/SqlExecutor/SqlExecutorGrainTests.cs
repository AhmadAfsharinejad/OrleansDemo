using System;
using System.Collections.Generic;
using System.Data.Odbc;
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

//TODO Test connection
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed class SqlExecutorGrainTests
{
    private readonly ISqlExecutorGrain _sut;
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<SqlExecutorConfig> _pluginConfigFetcher;
    private readonly ISqlExecutorService _sqlExecutorService;
    private readonly IFieldTypeJoiner _fieldTypeJoiner;

    public SqlExecutorGrainTests()
    {
        _pluginOutputCaller = Substitute.For<IPluginOutputCaller>();
        _pluginConfigFetcher = Substitute.For<IPluginConfigFetcher<SqlExecutorConfig>>();
        _sqlExecutorService = Substitute.For<ISqlExecutorService>();
        _fieldTypeJoiner = Substitute.For<IFieldTypeJoiner>();
        _sut = new SqlExecutorGrain(_pluginOutputCaller, _pluginConfigFetcher, _sqlExecutorService, _fieldTypeJoiner);
    }
    
    [Fact]
    public async Task Compute_ShouldCallFieldTypeJoinerOneTime_WhenCallComputeMultipleTime()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig();
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);

        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, null, tcs.Token);
        await _sut.Compute(pluginContext, null, tcs.Token);

        // Assert
        _fieldTypeJoiner.Received(1).Join(pluginContext.InputFieldTypes, 
            Arg.Is<IEnumerable<StreamField>>(x => x.SequenceEqual(pluginConfig.DqlCommand!.Value.OutputFileds.Select(z => z.Field))),
            pluginConfig.JoinType);
        
        _fieldTypeJoiner.ReceivedWithAnyArgs(1).Join(default, default, default);
    }

    [Fact]
    public async Task Compute_ShouldCallSqlExecutorService_WhenInputIsNull()
    {
        // Arrange
        var pluginContext = GetPluginContext();
        var pluginConfig = new SqlExecutorConfig();
        _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId).Returns(pluginConfig);

        // Act
        using var tcs = new GrainCancellationTokenSource();
        await _sut.Compute(pluginContext, null, tcs.Token);

        // Assert
        await foreach (var _ in _sqlExecutorService.Received(1)
                           .Execute(Arg.Any<OdbcConnection>(), pluginConfig, null,
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
                               .Execute(Arg.Any<OdbcConnection>(), pluginConfig, record,
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
        await _pluginOutputCaller.ReceivedWithAnyArgs(1).CallOutputs(default, default!, default!);
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
        await _sut.Compute(pluginContext, null, tcs.Token);

        // Assert
        await _pluginOutputCaller.ReceivedWithAnyArgs(1).CallOutputs(default, default!, default!);
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