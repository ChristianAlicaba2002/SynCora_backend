using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using syncora_server.Class;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.IUser;

namespace syncora_server.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<string> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        if (string.IsNullOrEmpty(_loginUserDTOs.Email) && string.IsNullOrEmpty(_loginUserDTOs.Password))
        {
            _logger.LogWarning("Login attempt rejected: email and password are missing");
            throw new UserExceptions.RequiredAllFields("All fields are required", StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrEmpty(_loginUserDTOs.Email))
        {
            _logger.LogWarning("Login attempt rejected: email is missing");
            throw new UserExceptions.EmailIsRequired("Email is required", StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrEmpty(_loginUserDTOs.Password))
        {
            _logger.LogWarning("Login attempt rejected: password is missing for {Email}", _loginUserDTOs.Email);
            throw new UserExceptions.RequiredAllFields("Password is required", StatusCodes.Status400BadRequest);
        }

        var user = await _userRepository.GetUserEmail(_loginUserDTOs.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(_loginUserDTOs.Password, user.Password))
        {
            _logger.LogWarning("Login failed for {Email}: Email or password is incorrect", _loginUserDTOs.Email);
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
            _logger.LogWarning("Registration rejected: email already in use for {Email}", registerUserDTOs.Email);
            throw new UserExceptions.EmailAlreadyUsed("Email is already used.", StatusCodes.Status409Conflict);
        }

        await _userRepository.RegisterUser(registerUserDTOs);
        _logger.LogInformation("User registered successfully for {Email}", registerUserDTOs.Email);
    }
}
