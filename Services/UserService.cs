using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using syncora_server.Class;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.IUser;

namespace syncora_server.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task<string> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        var user = await _userRepository.GetUserEmail(_loginUserDTOs.Email);
        
        if(string.IsNullOrEmpty(_loginUserDTOs.Email) && string.IsNullOrEmpty(_loginUserDTOs.Password))
        {
            throw new UserExceptions.RequiredAllFields("All fields are required", StatusCodes.Status400BadRequest);
        }

        if(string.IsNullOrEmpty(_loginUserDTOs.Email))
        {
            throw new UserExceptions.EmailIsRequired("Email is required", StatusCodes.Status400BadRequest);
        }
        
        if(string.IsNullOrEmpty(_loginUserDTOs.Password))
        {
            throw new UserExceptions.RequiredAllFields("Password is required", StatusCodes.Status400BadRequest);
        }

        if (user is null || !BCrypt.Net.BCrypt.Verify(_loginUserDTOs.Password, user.Password))
        {
            throw new UserExceptions.UserNotFound("Email or password is incorrect", StatusCodes.Status400BadRequest);
        }

        var jwt = _configuration.GetSection("jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("Role", Enums.UserRole.User.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task RegisterUser(UserDTOs.RegisterUserDTOs registerUserDTOs)
    {
        var existingUser = await _userRepository.GetUserEmail(registerUserDTOs.Email);

        if (existingUser is not null)
        {
            throw new UserExceptions.EmailAlreadyUsed("Email is already used.", StatusCodes.Status409Conflict);
        }

        await _userRepository.RegisterUser(registerUserDTOs);
    }
}
