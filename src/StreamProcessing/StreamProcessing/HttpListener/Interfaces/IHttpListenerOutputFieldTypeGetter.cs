using StreamProcessing.HttpListener.Domain;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpListener.Interfaces;

internal interface IHttpListenerOutputFieldTypeGetter
{
    IReadOnlyDictionary<string, FieldType> GetOutputs(HttpListenerConfig config);
}