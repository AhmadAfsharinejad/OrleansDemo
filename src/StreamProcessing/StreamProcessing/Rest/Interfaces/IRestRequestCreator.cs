using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Rest.Domain;

namespace StreamProcessing.Rest.Interfaces;

internal interface IRestRequestCreator
{
    HttpRequestMessage Create(RestConfig config, PluginRecord pluginRecord, CancellationToken cancellationToken);
}