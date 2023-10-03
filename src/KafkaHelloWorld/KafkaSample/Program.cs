// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;

Console.WriteLine($"Starting {DateTime.Now}");

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
};

using var producer = new ProducerBuilder<string, string>(config).Build();

var tasks = new List<Task>
{
    Produce(producer, 0),
    Produce(producer, 1),
    Produce(producer, 2),
    Produce(producer, 3),
    Produce(producer, 4)
};

await Task.WhenAll(tasks);

Console.WriteLine($"End {DateTime.Now}");
Console.ReadLine();

async Task Produce(IProducer<string, string> kafkaProducer, int index)
{
    const int max = 100000;
    var start = index * max;
    for (var i = start; i < start + max; i++)
    {
        await kafkaProducer.ProduceAsync("second-topic2", 
            new Message<string, string> { Key = $"k{i}", Value = $"v{i}" });
    }
}