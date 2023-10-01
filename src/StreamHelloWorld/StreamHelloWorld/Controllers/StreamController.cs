using Microsoft.AspNetCore.Mvc;
using StreamHelloWorld.Grains.Interfaces;

namespace StreamHelloWorld2.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class StreamController : ControllerBase
{
    private readonly IGrainFactory _grainFactory;

    public StreamController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Produce()
    {
        var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var grain = _grainFactory.GetGrain<IProducerGrain>(id);
        await grain.StartProducing(Guid.NewGuid());
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> Consume()
    {
        var grain = _grainFactory.GetGrain<IConsumerGrain>(Guid.NewGuid());
        return Ok();
    }
}