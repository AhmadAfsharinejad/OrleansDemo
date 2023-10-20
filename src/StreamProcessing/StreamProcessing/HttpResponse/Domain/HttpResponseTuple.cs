namespace StreamProcessing.HttpResponse.Domain;

[Immutable]
internal record struct HttpResponseTuple(byte[]? ContentBytes, IReadOnlyCollection<KeyValuePair<string, string>> Headers);
