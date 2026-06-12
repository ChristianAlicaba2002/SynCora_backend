using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.DTOs;
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
    public async Task<IActionResult> SendFollowRequest([FromBody] SendFollowRequestDto? dto)
    {
        if (dto is null || dto.ReceiverId == Guid.Empty)
            return BadRequest(new { message = "Receiver id is required.", status = StatusCodes.Status400BadRequest });

        try
        {
            await _followService.SendFollowRequestAsync(dto.ReceiverId);
            return Ok(new { message = "Follow request sent successfully.", status = StatusCodes.Status200OK });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpPost("accept")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> AcceptFollowRequest([FromBody] AcceptFollowRequestDto? dto)
    {
        if (dto is null || dto.RequestId == Guid.Empty)
            return BadRequest(new { message = "Request id is required.", status = StatusCodes.Status400BadRequest });

        try
        {
            await _followService.AcceptFollowRequestAsync(dto.RequestId);
            return Ok(new { message = "Follow request accepted successfully.", status = StatusCodes.Status200OK });
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = e.Message, status = StatusCodes.Status403Forbidden });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserFollowRequests()
    {
        try
        {
            var followRequests = await _followService.GetUserFollowRequestAsync();
            return Ok(new { message = "Follow requests retrieved successfully.", status = StatusCodes.Status200OK, data = followRequests });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, status = StatusCodes.Status400BadRequest });
        }
    }
}
