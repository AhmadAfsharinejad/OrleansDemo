namespace KafkaStreamHelloWorld.Grains.Interfaces;

public interface IConsumerGrain : IGrainWithStringKey
{
    Task Active();
}