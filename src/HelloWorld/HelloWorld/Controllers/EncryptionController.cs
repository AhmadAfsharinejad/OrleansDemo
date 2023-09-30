using HelloWorld.Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers;

[ApiController]
[Route("api/[controller]/[Action]")]
public class EncryptionController : ControllerBase
{
    private readonly IGrainFactory _grainFactory;

    public EncryptionController(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Encrypt([FromQuery] string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return BadRequest("Invalid value");
        }

        var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var grain = _grainFactory.GetGrain<IEncryptionGrain>(id);
        await grain.Encrypt(value);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> Decrypt([FromQuery] string encryptedValue)
    {
        if (string.IsNullOrWhiteSpace(encryptedValue))
        {
            return BadRequest("Invalid encryptedValue");
        }

        var entryGrain = _grainFactory.GetGrain<IEncryptionGrain>(encryptedValue);
        var result = await entryGrain.Decrypt();
        return Ok(result);
    }
}