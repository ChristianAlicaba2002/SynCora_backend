using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.IUser;
using syncora_server.Utils;

namespace syncora_server.Controller;

[ApiController]
[Route("api/v1/users")]
public class UserControllerController(IUserService iUserService, ILogger<UserControllerController> logger) : ControllerBase
{
    private readonly IUserService _iUserService = iUserService;
    private readonly ILogger<UserControllerController> _logger = logger;

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RegisterUser(
    UsersDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        try
        {
            await _iUserService.RegisterUser(_registerUserDTOs);

            _logger.LogInformation("User registered successfully for {Email}",_registerUserDTOs.Email);
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status201Created,"User Registered Successfully",null);
            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (UserExceptions.EmailAlreadyUsed e)
        {
            _logger.LogInformation("Email already used for {Email}", _registerUserDTOs.Email);
            var response = ApiResponse<object>.FailedResponse( StatusCodes.Status409Conflict,e.Message);
            return Conflict(response);
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
            var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "User Login Successfully", syncora_jwt );
            return Ok(response);
        }
        catch (UserExceptions.UserNotFound e)
        {
            _logger.LogInformation("User not found for {Email}", _loginUserDTOs.Email);
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status400BadRequest, e.Message);
            return BadRequest(response);
        }
        catch (UserExceptions.RequiredAllFields e)
        {
            _logger.LogInformation("All fields are required");
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status400BadRequest, e.Message);
            return BadRequest(response);
        }
        catch (UserExceptions.EmailIsRequired e)
        {
            _logger.LogInformation("Email is required");
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status400BadRequest, e.Message);
            return BadRequest(response);
        }
        catch (UserExceptions.PasswordIsRequired e)
        {
            _logger.LogInformation("Password is required");
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status400BadRequest, e.Message);
            return BadRequest(response);
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
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status404NotFound, "User not found.");
            return NotFound(response);
        }

        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "User retrieved successfully.", user);
        return Ok(response);
    }

    [Authorize(Policy = "UserOnly")]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UsersDTOs.UpdateUserDTO _updateUserDTOs)
    {
        await _iUserService.UpdateUserProfile(id, _updateUserDTOs);
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "User updated successfully.", null);
        return Ok(response);
    }

    [Authorize(Policy = "UserOnly")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _iUserService.GetUserById(id);
        if (user is null)
        {
            _logger.LogWarning("User not found for {UserId}", id);
            var response = ApiResponse<object>.FailedResponse(StatusCodes.Status404NotFound, "User not found.");
            return NotFound(response);
        }
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "User retrieved successfully.", user);
        return Ok(response);
    }

    [HttpGet("search")]
    [EnableRateLimiting("read")]
    public async Task<IActionResult> SearchUser(string searchQuery)
    {
        var users = await _iUserService.SearchUser(searchQuery);
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "Users searched successfully.", users);
        return Ok(response);
    }

    [HttpGet("{id:guid}/followers-count")]
    public async Task<IActionResult> GetUserFollowersCount(Guid id)
    {
        var followersCount = await _iUserService.GetUserFollowersCount(id);
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "User followers count retrieved successfully.", followersCount);
        return Ok(response);
    }

    [HttpGet("{id:guid}/following-count")]
    public async Task<IActionResult> GetUserFollowingCount(Guid id)
    {
        var followingCount = await _iUserService.GetUserFollowingCount(id);
        var response = ApiResponse<object>.SuccessResponse(StatusCodes.Status200OK, "User following count retrieved successfully.", followingCount);
        return Ok(response);
    }
}
