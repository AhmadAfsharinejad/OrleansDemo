using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Rest.Domain;

namespace StreamProcessing.Rest.Interfaces;

internal interface IUriReplacer
{
    string GetUri(RestConfig config, PluginRecord pluginRecord);
}