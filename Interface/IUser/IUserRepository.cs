
using syncora_server.DTOs;
using syncora_server.Models;

namespace syncora_server.Interface.IUser;

public interface IUserRepository
{
    Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs);
    Task<User?> GetUserEmail(string email);
}
