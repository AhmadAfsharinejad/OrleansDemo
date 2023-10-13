using System.Data.Odbc;
using System.Diagnostics.CodeAnalysis;
using Orleans.Concurrency;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.SqlExecutor.Domain;
using StreamProcessing.SqlExecutor.Interfaces;

namespace StreamProcessing.SqlExecutor;

[StatelessWorker]
[Reentrant]
[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed class SqlExecutorGrain : PluginGrain, ISqlExecutorGrain
{
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<SqlExecutorConfig> _pluginConfigFetcher;
    private readonly ISqlExecutorService _sqlExecutorService;
    private readonly IFieldTypeJoiner _fieldTypeJoiner;
    private OdbcConnection? _connection;
    private IReadOnlyDictionary<string, FieldType>? _outputFieldTypes;
    private bool _hasBeenInit;

    public SqlExecutorGrain(IPluginOutputCaller pluginOutputCaller,
        IPluginConfigFetcher<SqlExecutorConfig> pluginConfigFetcher,
        ISqlExecutorService sqlExecutorService,
        IFieldTypeJoiner fieldTypeJoiner)
    {
        _pluginOutputCaller = pluginOutputCaller ?? throw new ArgumentNullException(nameof(pluginOutputCaller));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _sqlExecutorService = sqlExecutorService ?? throw new ArgumentNullException(nameof(sqlExecutorService));
        _fieldTypeJoiner = fieldTypeJoiner ?? throw new ArgumentNullException(nameof(fieldTypeJoiner));
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"SqlExecutorGrain Activated  {this.GetGrainId()}");
        
        return base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        await Dispose();

        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    private async Task Dispose()
    {
        _hasBeenInit = false;

        if (_connection is null)
        {
            return;
        }

        await _connection.DisposeAsync();

        _connection = null;
        _outputFieldTypes = null;
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecords? pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);
        await Init(pluginContext.InputFieldTypes, config, cancellationToken.CancellationToken);
        var outPluginContext = pluginContext with { InputFieldTypes = _outputFieldTypes };

        List<PluginRecord> records;

        if (pluginRecords is null)
        {
            records = await Execute(null, config, cancellationToken.CancellationToken);
        }
        else
        {
            records = new List<PluginRecord>();

            foreach (var pluginRecord in pluginRecords.Value.Records)
            {
                var result = await Execute(pluginRecord, config, cancellationToken.CancellationToken);
                records.AddRange(result);
            }
        }

        await _pluginOutputCaller.CallOutputs(outPluginContext, records, cancellationToken);
    }

    private async Task<List<PluginRecord>> Execute(PluginRecord? record, SqlExecutorConfig config, CancellationToken cancellationToken)
    {
        var records = new List<PluginRecord>();

        await foreach (var readRecord in _sqlExecutorService.Execute(_connection!, config, record, cancellationToken))
        {
            records.Add(readRecord);
        }

        return records;
    }

    private async Task Init(IReadOnlyDictionary<string, FieldType>? inputFieldTypesByName,
        SqlExecutorConfig config,
        CancellationToken cancellationToken)
    {
        if (_hasBeenInit) return;

        await InitConnection(config, cancellationToken);
        InitOutputFieldTypes(inputFieldTypesByName, config);

        _hasBeenInit = true;
    }

    private async Task InitConnection(SqlExecutorConfig config, CancellationToken cancellationToken)
    {
        _connection = new OdbcConnection(config.ConnectionString);
        await _connection.OpenAsync(cancellationToken);
    }

    private void InitOutputFieldTypes(IReadOnlyDictionary<string, FieldType>? inputFieldTypesByName, SqlExecutorConfig config)
    {
        _outputFieldTypes = _fieldTypeJoiner.Join(inputFieldTypesByName,
            config.DqlCommand?.OutputFileds.Select(x => x.Field),
            config.JoinType);
    }
}