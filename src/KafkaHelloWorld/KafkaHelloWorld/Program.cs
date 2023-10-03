using KafkaHelloWorld;
using KafkaHelloWorld.Domains;
using Orleans.Streams.Kafka.Config;

var hostBuilder = new HostBuilder()
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering()
            .AddMemoryGrainStorageAsDefault()
            .AddMemoryGrainStorage(Consts.PubSubStore)
            .AddKafka(Consts.StreamProvider)
            .WithOptions(options =>
            {
                options.BrokerList = new[] { "localhost:9092" };
                options.ConsumerGroupId = "E2EGroup2";
                options.ConsumeMode = ConsumeMode.LastCommittedMessage;
                options.MessageTrackingEnabled = false;
                options.PollTimeout = TimeSpan.FromMinutes(1);
        
                options
                    .AddTopic(Consts.FirstTopic);
            })
            //.AddJson()
            .AddLoggingTracker()
            .Build();
    });

hostBuilder.ConfigureServices(services => 
    services.AddHostedService<ProducerHost>()
    .AddHostedService<ConsumerHost>());

var host = hostBuilder.Build();
await host.StartAsync();
Console.WriteLine("Press enter to stop the Silo...");
Console.ReadLine();
await host.StopAsync();