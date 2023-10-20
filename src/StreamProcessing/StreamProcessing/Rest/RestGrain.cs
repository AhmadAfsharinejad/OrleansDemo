using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest;

[StatelessWorker]
[Reentrant]
internal sealed class RestGrain : Grain, IRestGrain
{
    private readonly IPluginOutputCaller _pluginOutputCaller;
    private readonly IPluginConfigFetcher<RestConfig> _pluginConfigFetcher;
    private readonly IRestService _restService;
    private readonly IFieldTypeJoiner _fieldTypeJoiner;
    private HttpClient? _httpClient;
    private IReadOnlyDictionary<string, FieldType>? _outputFieldTypes;

    public RestGrain(IPluginOutputCaller pluginOutputCaller,
        IPluginConfigFetcher<RestConfig> pluginConfigFetcher,
        IRestService restService,
        IFieldTypeJoiner fieldTypeJoiner)
    {
        _pluginOutputCaller = pluginOutputCaller ?? throw new ArgumentNullException(nameof(pluginOutputCaller));
        _pluginConfigFetcher = pluginConfigFetcher ?? throw new ArgumentNullException(nameof(pluginConfigFetcher));
        _restService = restService ?? throw new ArgumentNullException(nameof(restService));
        _fieldTypeJoiner = fieldTypeJoiner ?? throw new ArgumentNullException(nameof(fieldTypeJoiner));
    }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"RestGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        Dispose();

        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    private void Dispose()
    {
        if (_httpClient is null)
        {
            return;
        }

        _httpClient.Dispose();
        _httpClient = null;
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecords pluginRecords,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);
        Init(pluginContext, config);

        var records = new List<PluginRecord>(pluginRecords.Records.Count);

        foreach (var pluginRecord in pluginRecords.Records)
        {
            var record = await _restService.Call(_httpClient!, config, pluginRecord, cancellationToken.CancellationToken);

            records.Add(record);
        }

        await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), records, cancellationToken);
    }

    [ReadOnly]
    public async Task Compute([Immutable] PluginExecutionContext pluginContext,
        [Immutable] PluginRecord pluginRecord,
        GrainCancellationToken cancellationToken)
    {
        var config = await _pluginConfigFetcher.GetConfig(pluginContext.ScenarioId, pluginContext.PluginId);
        Init(pluginContext, config);

        var record = await _restService.Call(_httpClient!, config, pluginRecord, cancellationToken.CancellationToken);

        await _pluginOutputCaller.CallOutputs(GetOutPluginContext(pluginContext), record, cancellationToken);
    }

    private PluginExecutionContext GetOutPluginContext(PluginExecutionContext pluginContext)
    {
        return pluginContext with { InputFieldTypes = _outputFieldTypes };
    }

    private void Init(PluginExecutionContext pluginContext, RestConfig config)
    {
        if (_httpClient is not null)
        {
            return;
        }

        InitOutputFieldTypes(pluginContext, config);

        _httpClient = new HttpClient();
    }

    private void InitOutputFieldTypes(PluginExecutionContext pluginContext, RestConfig config)
    {
        var outputs = new List<StreamField>();

        if (config.ResponseHeaders is not null)
        {
            foreach (var requestHeader in config.ResponseHeaders)
            {
                outputs.Add(new StreamField(requestHeader.FieldName, FieldType.Text));
            }
        }

        if (!string.IsNullOrWhiteSpace(config.StatusFieldName))
        {
            outputs.Add(new StreamField(config.StatusFieldName, FieldType.Integer));
        }
        
        if (!string.IsNullOrWhiteSpace(config.ResponseContentFieldName))
        {
            outputs.Add(new StreamField(config.ResponseContentFieldName, FieldType.Text));
        }

        _outputFieldTypes = _fieldTypeJoiner.Join(pluginContext.InputFieldTypes, outputs, config.JoinType);
    }
}