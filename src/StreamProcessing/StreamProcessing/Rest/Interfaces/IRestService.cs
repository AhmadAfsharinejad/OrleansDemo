using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Rest.Domain;

namespace StreamProcessing.Rest.Interfaces;

internal interface IRestService
{
    Task<PluginRecord> Call(HttpClient httpClient,
        RestConfig config,
        PluginRecord pluginRecord,
        CancellationToken cancellationToken);
}