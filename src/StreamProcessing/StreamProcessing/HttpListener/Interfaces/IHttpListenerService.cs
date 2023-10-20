using StreamProcessing.HttpListener.Domain;

namespace StreamProcessing.HttpListener.Interfaces;

internal interface IHttpListenerService
{
    IAsyncEnumerable<RecordListenerContextTuple> Listen(HttpListenerConfig config, CancellationToken cancellationToken);
}