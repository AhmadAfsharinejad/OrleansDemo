using Microsoft.AspNetCore.Mvc;

namespace Rest.Sample.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MathController : ControllerBase
{
    public int Add([FromQuery] int a, [FromQuery] int b)
    {
        return a + b;
    }
}