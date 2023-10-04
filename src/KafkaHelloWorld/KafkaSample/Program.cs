// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;

Console.WriteLine($"Starting {DateTime.Now}");

//await Write();

var t1 = Task.Run(async () => { await Read(); });
// var t2 = Task.Run(async () => { await Read(); });
// var t3 = Task.Run(async () => { await Read(); });

await Task.WhenAll(t1);

Console.WriteLine($"End {DateTime.Now}");
Console.ReadLine();

async Task Write()
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092"
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
}

async Task Produce(IProducer<string, string> producer, int index)
{
    const int max = 100000;
    var start = index * max;
    for (var i = start; i < start + max; i++)
    {
        await producer.ProduceAsync("second-topic2",
            new Message<string, string> { Key = $"k{i}", Value = $"v{i}" });
    }
}

async Task Read()
{
    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9092",
        GroupId = "g6",
        EnableAutoCommit = false,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnablePartitionEof = true
    };

    using var consumer = new ConsumerBuilder<string, string>(config).Build();
    //consumer.Subscribe(new[] { "second-topic2" });
    consumer.Assign(new TopicPartitionOffset("second-topic2", 0, Offset.Beginning));


    int count = 0;
    while (true)
    {
        var result = consumer.Consume();
        count++;
        //Console.WriteLine(result?.Message?.Value);
        if (result?.IsPartitionEOF ?? true) break;
    }

    Console.WriteLine($"Total: {count}");
}