using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpResponse.Interfaces;

internal interface IHttpResponseService
{
    HttpResponseTuple GetResponse(HttpResponseConfig config, PluginRecord record);
}