namespace KafkaStreamHelloWorld.Grains.Interfaces;

public interface IProducerGrain : IGrainWithStringKey
{
    Task StartProducing();
}