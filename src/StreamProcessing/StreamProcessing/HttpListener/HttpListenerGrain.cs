using Orleans.Concurrency;
using StreamProcessing.HttpListener.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Silo.Interfaces;

namespace StreamProcessing.HttpListener;

[StatelessWorker]
internal sealed class HttpListenerGrain : Grain, IHttpListenerGrain
{
    private readonly IEachSiloCaller _eachSiloCaller;

    public HttpListenerGrain(IEachSiloCaller eachSiloCaller)
    {
        _eachSiloCaller = eachSiloCaller ?? throw new ArgumentNullException(nameof(eachSiloCaller));
    }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"HttpListenerGrain Activated {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }

    [ReadOnly]
    public async Task Start([Immutable] PluginExecutionContext pluginContext,
        GrainCancellationToken cancellationToken)
    {
        await _eachSiloCaller.Start(typeof(IHttpListenerLocalGrain), pluginContext, cancellationToken);
    }
}