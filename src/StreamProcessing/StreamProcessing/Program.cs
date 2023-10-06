﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreamProcessing;
using StreamProcessing.Di;

var hostBuilder = new HostBuilder()
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
        siloBuilder.AddMemoryGrainStorage("Storage");
        siloBuilder.Services.AddStreamServices();
        //siloBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
    });

hostBuilder.ConfigureServices(services => 
    services.AddHostedService<StartingHost>());

var host = hostBuilder.Build();
await host.StartAsync();
Console.WriteLine("Press enter to stop the Silo...");
Console.ReadLine();
await host.StopAsync();