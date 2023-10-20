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
    private readonly IConnectionFactory _connectionFactory;
    private readonly ISqlExecutorService _sqlExecutorService;
    private readonly IFieldTypeJoiner _fieldTypeJoiner;
    private IStreamDbConnection? _connection;
    private IStreamDbCommand? _command;
    private IReadOnlyDictionary<string, FieldType>? _outputFieldTypes;
    private bool _hasBeenInit;

    public SqlExecutorGrain(IPluginOutputCaller pluginOutputCaller,
        IPluginConfigFetcher<SqlExecutorConfig> pluginConfigFetcher,
        IConnectionFactory connectionFactory,
        ISqlExecutorService sqlExecutorService,
        IFieldTypeJoiner fieldTypeJoiner)
    {
        _pluginOutputCaller = pluginOutputCaller ?? throw new ArgumentNullException(nameof(pluginOutputCaller));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
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

        _command!.Dispose();
        await _connection.DisposeAsync();

        _command = null;
        _connection = null;
        _outputFieldTypes = null;
    }

    [ReadOnly]
    public async Task Start([Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken)
    {
        var config = await Init(pluginContext, cancellationToken.CancellationToken);

        var records = await Execute(null, config, cancellationToken.CancellationToken);
        await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), records, cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecords pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        var config = await Init(pluginContext, cancellationToken.CancellationToken);

        var records = new List<PluginRecord>();

        foreach (var pluginRecord in pluginRecords.Records)
        {
            var result = await Execute(pluginRecord, config, cancellationToken.CancellationToken);
            records.AddRange(result);
        }

        await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), records, cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord pluginRecord,
        GrainCancellationToken cancellationToken)
    {
        var config = await Init(pluginContext, cancellationToken.CancellationToken);

        var records = await Execute(pluginRecord, config, cancellationToken.CancellationToken);

        if (records.Count > 1)
        {
            await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), records, cancellationToken);
        }
        else
        {
            await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), records.First(), cancellationToken);
        }
    }

    private async Task<List<PluginRecord>> Execute(PluginRecord? record, SqlExecutorConfig config, CancellationToken cancellationToken)
    {
        var records = new List<PluginRecord>();

        await foreach (var readRecord in _sqlExecutorService.Execute(_connection!, _command!, config, record, cancellationToken))
        {
            records.Add(readRecord);
        }

        return records;
    }

    private PluginExecutionContext GetOutPluginContext(PluginExecutionContext pluginContext)
    {
        return pluginContext with { InputFieldTypes = _outputFieldTypes };
    }

    private async Task<SqlExecutorConfig> Init(PluginExecutionContext pluginContext, CancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);
        await Init(pluginContext, config, cancellationToken);
        return config;
    }

    private async Task Init(PluginExecutionContext pluginContext,
        SqlExecutorConfig config,
        CancellationToken cancellationToken)
    {
        if (_hasBeenInit) return;

        await InitConnection(config, cancellationToken);

        _command = _connection!.CreateStreamDbCommand();

        InitOutputFieldTypes(pluginContext, config);

        _hasBeenInit = true;
    }

    private async Task InitConnection(SqlExecutorConfig config, CancellationToken cancellationToken)
    {
        _connection = _connectionFactory.Create(config.ConnectionString);
        await _connection.OpenAsync(cancellationToken);
    }

    private void InitOutputFieldTypes(PluginExecutionContext pluginContext, SqlExecutorConfig config)
    {
        _outputFieldTypes = _fieldTypeJoiner.Join(pluginContext.InputFieldTypes,
            config.DqlCommand?.OutputFields.Select(x => x.Field),
            config.JoinType);
    }
}