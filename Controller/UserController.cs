using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.IUser;

namespace syncora_server.Controller;

[ApiController]
[Route("api/v1/users")]
public class UserControllerController(IUserService iUserService) : ControllerBase
{
    private readonly IUserService _iUserService = iUserService;

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        try
        {
            await _iUserService.RegisterUser(_registerUserDTOs);
            return Ok(new { message = "User Registered Successfully", status = StatusCodes.Status201Created });
        }
        catch (UserExceptions.EmailAlreadyUsed e)
        {
            return Conflict(new { message = e.Message, status = e.Status });
        }
    }
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        try
        {
            var jwt = await _iUserService.LoginUser(_loginUserDTOs);
            return Ok(new { message = "User Login Successfully", status = StatusCodes.Status200OK, jwt = jwt });
        }
        catch (UserExceptions.UserNotFound e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
        catch (UserExceptions.RequiredAllFields e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
        catch(UserExceptions.EmailIsRequired e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
        catch(UserExceptions.PasswordIsRequired e)
        {
            return BadRequest(new { message = e.Message, status = e.Status });
        }
    }
}
