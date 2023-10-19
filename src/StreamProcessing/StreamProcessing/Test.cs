using System.Net;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Runtime.Services;
using Orleans.Services;

namespace StreamProcessing;

public class Test : GrainService, IExampleGrainService
{
    readonly IGrainFactory _grainFactory;

    public Test(
        IServiceProvider services,
        GrainId id,
        Orleans.Runtime.Silo silo,
        ILoggerFactory loggerFactory,
        IGrainFactory grainFactory)
        : base(id, silo, loggerFactory)
    {
        _grainFactory = grainFactory;
    }

    public override Task Init(IServiceProvider serviceProvider)
    {
        var grain = _grainFactory.GetGrain<IClientListener>(Guid.NewGuid());
        grain.Run();
        return base.Init(serviceProvider);
    }

    public override Task Start()
    {
        return base.Start();
    }

    public Task DoSomething()
    {
        return Task.CompletedTask;
    }
}

public interface IExampleGrainService : IGrainService
{
    Task DoSomething();
}

public interface IClientListener : IGrainWithGuidKey
{
    Task Run();
}

public class ClientListener : Grain, IClientListener
{
    public async Task Run()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:1380/index/");
        listener.Start();
        Listen(listener);
    }

    async Task Listen(HttpListener listener)
    {
        while (listener.IsListening)
        {
            var context = listener.GetContext();
            Console.WriteLine($"{context.Request.Url}");
            
            var response = context.Response;

            var responseString = "Ur response";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            await using var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }
    }
}