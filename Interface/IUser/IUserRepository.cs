
using syncora_server.DTOs;
using syncora_server.Models;

namespace syncora_server.Interface.IUser;

public interface IUserRepository
{
    Task RegisterUser(UsersDTOs.RegisterUserDTOs _registerUserDTOs);
    Task<User?> GetUserEmail(string email);
    Task<User?> GetUserById(Guid id);
    Task UpdateUserProfile(UsersDTOs.UpdateUserDTO _updateUserDTO, Guid userId);
    Task<List<UsersDTOs.UserProfileDTO>> SearchUser(string searchQuery);
    Task<bool> IsFollowing(Guid followerId, Guid followingId);
    Task<bool> IsRequested(Guid senderId, Guid receiverId);
    Task<bool> HasIncomingRequest(Guid receiverId, Guid senderId);
    Task<int> GetUserFollowersCount(Guid followerId);
    Task<int> GetUserFollowingCount(Guid followingId);
}
