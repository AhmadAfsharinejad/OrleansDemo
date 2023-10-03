using KafkaHelloWorld;
using KafkaHelloWorld.Domains;
using KafkaHelloWorld.Serialization;
using Orleans.Streams.Kafka.Config;

var hostBuilder = new HostBuilder()
    .UseOrleans(siloBuilder =>
    {
        //siloBuilder.ConfigureLogging(logginBuilder => logginBuilder.AddConsole());
        siloBuilder.UseLocalhostClustering()
            .AddMemoryGrainStorageAsDefault()
            .AddMemoryGrainStorage(Consts.PubSubStore)
            .AddKafka(Consts.StreamProvider)
            .WithOptions(options =>
            {
                options.BrokerList = new[] { "localhost:9092" };
                options.ConsumerGroupId = "E2EGroup7";
                options.ConsumeMode = ConsumeMode.StreamStart;//Note: When internal change to StreamEnd
                options.MessageTrackingEnabled = false;
                //options.PollTimeout = TimeSpan.FromSeconds(1);

                options
                    //.AddTopic(Consts.InternalTopic);
                .AddExternalTopic<object>(Consts.ExternalTopic);
            })
            //.AddJson()
            .AddExternalDeserializer<ExternalStreamDeserializer>()
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