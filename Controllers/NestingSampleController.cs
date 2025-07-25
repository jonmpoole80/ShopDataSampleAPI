using Microsoft.AspNetCore.Mvc;
using ShopDataSampleAPI.Models;

namespace ShopDataSampleAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NestingSampleController : ControllerBase
{
    [HttpPost("NEST")]
    public IActionResult Nest([FromBody] NestingRequest request)
    {
        var response = new NestingResponse
        {
            Input = request.Input,
            Output = "Hello world"
        };

        return Ok(response);
        // This is where we would call the Nesting code from C and
        // then return the nesting data formed as a valid JSON result.
        // I have added some details to the README.md file to explain
        // the process. -- JP 7.24.25
    }
}
