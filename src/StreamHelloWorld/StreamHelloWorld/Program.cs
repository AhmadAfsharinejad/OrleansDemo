using Orleans.Providers;
using StreamHelloWorld.Domains;
using StreamHelloWorld;

var hostBuilder = new HostBuilder()
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.ConfigureLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryGrainStorage(Consts.PubSubStore);
        siloBuilder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>(Consts.StreamProvider);
    });

hostBuilder.ConfigureServices(services => services.AddHostedService<StartingHost>());

var host = hostBuilder.Build();
await host.StartAsync();
Console.WriteLine("Press enter to stop the Silo...");
Console.ReadLine();
await host.StopAsync();