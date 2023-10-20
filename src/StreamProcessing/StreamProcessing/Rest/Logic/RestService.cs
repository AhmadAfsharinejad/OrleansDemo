using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest.Logic;

internal sealed class RestService : IRestService
{
    private readonly IRestRequestCreator _restRequestCreator;
    private readonly IRestResponseConverter _responseConverter;
    private readonly IRecordJoiner _recordJoiner;

    public RestService(IRestRequestCreator restRequestCreator,
        IRestResponseConverter responseConverter,
        IRecordJoiner recordJoiner)
    {
        _restRequestCreator = restRequestCreator ?? throw new ArgumentNullException(nameof(restRequestCreator));
        _responseConverter = responseConverter ?? throw new ArgumentNullException(nameof(responseConverter));
        _recordJoiner = recordJoiner ?? throw new ArgumentNullException(nameof(recordJoiner));
    }

    public async Task<PluginRecord> Call(HttpClient httpClient,
        RestConfig config,
        PluginRecord pluginRecord,
        CancellationToken cancellationToken)
    {
        using var request = _restRequestCreator.Create(config, pluginRecord, cancellationToken);
        
        var response = await httpClient.SendAsync(request, cancellationToken);

        var record = await _responseConverter.Convert(response, config);

        return _recordJoiner.Join(pluginRecord, record, config.JoinType);
    }
}