using syncora_server.DTOs;

namespace syncora_server.Interface.IUser;

public interface IUserService
{
    Task RegisterUser(UsersDTOs.RegisterUserDTOs _registerUserDTOs);
    Task<string> LoginUser(UsersDTOs.LoginUserDTOs _loginUserDTOs);
    Task<UsersDTOs.UserProfileDTO?> GetCurrentUser();
    Task UpdateUserProfile(Guid userId, UsersDTOs.UpdateUserDTO _updateUserDTO);
}
