namespace StreamHelloWorld.Grains.Interfaces;

public interface IProducerGrain : IGrainWithStringKey
{
    Task StartProducing(Guid key);

    Task StopProducing();
}