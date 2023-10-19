using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using StreamProcessing;
using StreamProcessing.Di;
using StreamProcessing.Silo;


var host = CreateHost(12111, 30000).Build();
await host.StartAsync();


var host2 = CreateHost(12112, 30001).Build();
await host2.StartAsync();


Console.WriteLine("Press enter to stop the Silo...");
Console.ReadLine();
await host.StopAsync();

IHostBuilder CreateHost(int siloPort, int gatewayPort)
{
    var hostBuilder = new HostBuilder()
        .UseOrleans(siloBuilder =>
        {
            siloBuilder.UseLocalhostClustering();
            siloBuilder.AddMemoryGrainStorage(SiloConsts.StorageName);
            siloBuilder.Services.AddStreamServices();
            //siloBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole());
            siloBuilder.AddGrainService<Test>();

            siloBuilder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "ServiceApp";
            });
            siloBuilder.UseRedisClustering(opt =>
            {
                opt.ConnectionString = "localhost:6379";
                opt.Database = 1;
            });
            siloBuilder.ConfigureEndpoints(siloPort: siloPort, gatewayPort: gatewayPort);
        });
    hostBuilder.ConfigureServices(services => { services.AddHostedService<StartingHost>(); });

    return hostBuilder;
}