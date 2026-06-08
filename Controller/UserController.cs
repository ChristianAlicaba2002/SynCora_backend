using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.IUser;

namespace syncora_server.Controller;

[ApiController]
[Route("api/v1/users")]
public class UserControllerController(IUserService iUserService, ILogger<UserControllerController> logger) : ControllerBase
{
    private readonly IUserService _iUserService = iUserService;
    private readonly ILogger<UserControllerController> _logger = logger;

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RegisterUser(UsersDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        try
        {
            await _iUserService.RegisterUser(_registerUserDTOs);
            _logger.LogInformation("User registered successfully for {Email}", _registerUserDTOs.Email);
            return Ok(new { message = "User Registered Successfully", status = StatusCodes.Status201Created });
        }
        catch (UserExceptions.EmailAlreadyUsed e)
        {
            return Conflict(new { message = e.Message, status = e.Status });
        }
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> LoginUser(UsersDTOs.LoginUserDTOs _loginUserDTOs)
    {
        try
        {
            var syncora_jwt = await _iUserService.LoginUser(_loginUserDTOs);
            _logger.LogInformation("User logged in successfully for {Email}", _loginUserDTOs.Email);
            return Ok(new { message = "User Login Successfully", status = StatusCodes.Status200OK, jwt = syncora_jwt });
        }
        catch (UserExceptions.UserNotFound e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
        catch (UserExceptions.RequiredAllFields e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
        catch (UserExceptions.EmailIsRequired e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
        catch (UserExceptions.PasswordIsRequired e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
    }

    [Authorize(Policy = "UserOnly")]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _iUserService.GetCurrentUser();
        if (user is null)
        {
            _logger.LogWarning("User not found for {UserId}", user?.Id);
            return NotFound(new { message = "User not found.", status = StatusCodes.Status404NotFound });
        }

        return Ok(new { message = "User retrieved successfully.", status = StatusCodes.Status200OK, data = user });
    }

    [Authorize(Policy = "UserOnly")]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UsersDTOs.UpdateUserDTO _updateUserDTOs)
    {
        await _iUserService.UpdateUserProfile(id, _updateUserDTOs);
        return Ok(new { message = "User updated successfully.", status = StatusCodes.Status200OK });
    }
}
