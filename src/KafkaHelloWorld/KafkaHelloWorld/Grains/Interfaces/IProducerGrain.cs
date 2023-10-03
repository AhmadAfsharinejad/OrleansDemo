namespace KafkaHelloWorld.Grains.Interfaces;

public interface IProducerGrain : IGrainWithStringKey
{
    Task StartProducing();
}