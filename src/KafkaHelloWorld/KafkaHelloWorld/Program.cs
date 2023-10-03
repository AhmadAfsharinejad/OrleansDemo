using KafkaHelloWorld;
using KafkaHelloWorld.Domains;
using Orleans.Providers;
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
                options.MessageTrackingEnabled = true;
                options.PollTimeout = TimeSpan.FromMinutes(1);
        
                options
                    .AddTopic(Consts.FirstTopic);
            })
            //.AddJson()
            .AddLoggingTracker()
            .Build();
        
        
        // siloBuilder.UseLocalhostClustering();
        // siloBuilder.ConfigureLogging(logging =>
        // {
        //     logging.AddConsole();
        //     logging.SetMinimumLevel(LogLevel.Information);
        // });
        // siloBuilder.AddMemoryGrainStorage(Consts.PubSubStore);
        // siloBuilder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>(Consts.StreamProvider);
    });

hostBuilder.ConfigureServices(services => 
    services.AddHostedService<ProducerHost>()
    .AddHostedService<ConsumerHost>());

var host = hostBuilder.Build();
await host.StartAsync();
Console.WriteLine("Press enter to stop the Silo...");
Console.ReadLine();
await host.StopAsync();