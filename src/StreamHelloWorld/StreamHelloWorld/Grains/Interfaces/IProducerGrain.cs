namespace StreamHelloWorld.Grains.Interfaces;

public interface IProducerGrain : IGrainWithStringKey
{
    Task StartProducing(string ns, Guid key);

    Task StopProducing();
}