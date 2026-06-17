using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using syncora_server.Class;
using syncora_server.DTOs;
using syncora_server.Exceptions;
using syncora_server.Interface.ICurrentUser;
using syncora_server.Interface.IUser;

namespace syncora_server.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger, ICurrentUserService currentUser) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<UserService> _logger = logger;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<string> LoginUser(UsersDTOs.LoginUserDTOs _loginUserDTOs)
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
            throw new UserExceptions.PasswordIsRequired("Password is required", StatusCodes.Status400BadRequest);
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

    public async Task RegisterUser(UsersDTOs.RegisterUserDTOs registerUserDTOs)
    {
        var existingUser = await _userRepository.GetUserEmail(registerUserDTOs.Email);

        if (existingUser is not null)
        {
            _logger.LogWarning("Registration rejected: email already in use for {Email}", registerUserDTOs.Email);
            throw new UserExceptions.EmailAlreadyUsed("Email is already used.", StatusCodes.Status409Conflict);
        }

        await _userRepository.RegisterUser(registerUserDTOs);
    }

    public async Task<UsersDTOs.UserProfileDTO?> GetCurrentUser()
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {
            _logger.LogWarning("Get current user attempt rejected: user is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var user = await _userRepository.GetUserById(_currentUser.UserId.Value);

        if (user is null) return null;

        return new UsersDTOs.UserProfileDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Gender = user.Gender,
            Email = user.Email,
            Bio = user.Bio,
            ImageUrl = user.ImageUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateddAt
        };
    }

    public async Task  UpdateUserProfile(Guid userId, UsersDTOs.UpdateUserDTO _updateUserDTO)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
        {   
            _logger.LogWarning("Update user profile attempt rejected: user is not authenticated for {UserId}", userId);
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        if (userId != _currentUser.UserId.Value)
        {
            _logger.LogWarning("Update user profile attempt rejected: user is not authorized to update this profile for {UserId}", userId);
            throw new UnauthorizedAccessException("User is not authorized to update this profile.");
        }

        await _userRepository.UpdateUserProfile(_updateUserDTO, userId);
    }

    public Task<List<UsersDTOs.UserProfileDTO>> SearchUser(string searchQuery)
    {
        return _userRepository.SearchUser(searchQuery);
    }

    public async Task<UsersDTOs.UserProfileDTO?> GetUserById(Guid id)
    {
        var user = await _userRepository.GetUserById(id);
        if (user is null) return null;

        var isFollowing = false;
        var isRequested = false;
        var hasIncomingRequest = false;

        if (_currentUser.IsAuthenticated && _currentUser.UserId is not null)
        {
            var currentUserId = _currentUser.UserId.Value;
            isFollowing = await _userRepository.IsFollowing(currentUserId, id);
            isRequested = await _userRepository.IsRequested(currentUserId, id);
            hasIncomingRequest = await _userRepository.HasIncomingRequest(currentUserId, id);
        }

        return new UsersDTOs.UserProfileDTO
        {
            Id = user.Id,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Gender = user.Gender,
            Email = user.Email,
            Bio = user.Bio,
            ImageUrl = user.ImageUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdateddAt,
            IsFollowing = isFollowing,
            IsRequested = isRequested,
            HasIncomingRequest = hasIncomingRequest
        };
    }

    public async Task<int> GetUserFollowersCount(Guid followerId)
    {
        return await _userRepository.GetUserFollowersCount(followerId);
    }

    public async Task<int> GetUserFollowingCount(Guid followingId)
    {
        return await _userRepository.GetUserFollowingCount(followingId);
    }
}
