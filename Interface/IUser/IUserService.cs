using syncora_server.DTOs;
using syncora_server.Models;

namespace syncora_server.Interface.IUser;

public interface IUserService
{
    Task RegisterUser(UsersDTOs.RegisterUserDTOs _registerUserDTOs);
    Task<string> LoginUser(UsersDTOs.LoginUserDTOs _loginUserDTOs);
    Task<UsersDTOs.UserProfileDTO?> GetCurrentUser();
    Task UpdateUserProfile(Guid userId, UsersDTOs.UpdateUserDTO _updateUserDTO);
    Task<List<UsersDTOs.UserProfileDTO>> SearchUser(string searchQuery);
}
