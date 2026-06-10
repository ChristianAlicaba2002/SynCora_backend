using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.Interface.IFollow;

namespace syncora_server.Controller;

[Authorize(Policy = "UserOnly")]
[ApiController]
[Route("api/v1/follows")]
public class FollowController(IFollowService followService) : ControllerBase
{
    private readonly IFollowService _followService = followService;

    [HttpPost("send")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> SendFollowRequest([FromBody] Guid receiverId)
    {
        try
        {
            await _followService.SendFollowRequestAsync(receiverId);
            return Ok(new { message = "Follow request sent successfully.", status = StatusCodes.Status200OK });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpPost("accept")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> AcceptFollowRequest([FromBody] Guid requestId)
    {
        try
        {
            await _followService.AcceptFollowRequestAsync(requestId);
            return Ok(new { message = "Follow request accepted successfully.", status = StatusCodes.Status200OK });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, status = StatusCodes.Status400BadRequest });
        }
    }

}