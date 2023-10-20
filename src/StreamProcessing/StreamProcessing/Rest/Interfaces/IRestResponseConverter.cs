using StreamProcessing.Rest.Domain;

namespace StreamProcessing.Rest.Interfaces;

internal interface IRestResponseConverter
{
    Task<IReadOnlyDictionary<string, object>> Convert(HttpResponseMessage responseMessage, RestConfig config);
}