using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamProcessing;

var hostBuilder = new HostBuilder()
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryGrainStorage("Storage");
        //siloBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
    });

hostBuilder.ConfigureServices(services => 
    services.AddHostedService<StartingHost>());

var host = hostBuilder.Build();
await host.StartAsync();
Console.WriteLine("Press enter to stop the Silo...");
Console.ReadLine();
await host.StopAsync();