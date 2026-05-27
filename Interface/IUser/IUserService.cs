using syncora_server.DTOs;

namespace syncora_server.Interface.IUser;

public interface IUserService
{
    Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs);
    Task<string> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs);
}
