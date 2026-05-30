using Microsoft.AspNetCore.Mvc;

namespace syncora_server.Controller;

[ApiController]
[Route("api/v1/tasks")]
public class TaskControllerController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
