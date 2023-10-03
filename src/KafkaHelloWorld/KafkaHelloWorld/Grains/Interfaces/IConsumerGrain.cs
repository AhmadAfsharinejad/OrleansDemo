namespace KafkaHelloWorld.Grains.Interfaces;

public interface IConsumerGrain : IGrainWithStringKey
{
    Task Active();
}