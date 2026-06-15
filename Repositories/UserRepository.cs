using Microsoft.EntityFrameworkCore;
using syncora_server.Data;
using syncora_server.DTOs;
using syncora_server.Interface.IUser;
using syncora_server.Models;

namespace syncora_server.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    private static string Capitalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        value = value.Trim();
        return char.ToUpper(value[0]) + value.Substring(1).ToLower();
    }

    public async Task<User?> GetUserEmail(string email)
    {
        var userEmail = await _context.Users.FirstOrDefaultAsync(user => user.Email == email);

        return userEmail;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task RegisterUser(UsersDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        var saveUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = Capitalize(_registerUserDTOs.FirstName),
            MiddleName = Capitalize(_registerUserDTOs.MiddleName),
            LastName = Capitalize(_registerUserDTOs.LastName),
            Gender = Capitalize(_registerUserDTOs.Gender),
            Email = _registerUserDTOs.Email.Trim(),
            Password = BCrypt.Net.BCrypt.HashPassword(_registerUserDTOs.Password.Trim()),
            CreatedAt = DateTime.Now,
            UpdateddAt = DateTime.Now,
        };

        _context.Users.Add(saveUser);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserProfile(UsersDTOs.UpdateUserDTO _updateUserDTO, Guid userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return;

        user.FirstName = Capitalize(_updateUserDTO.FirstName) is "" ? user.FirstName : Capitalize(_updateUserDTO.FirstName);
        user.MiddleName = _updateUserDTO.MiddleName ?? user.MiddleName;
        user.LastName = Capitalize(_updateUserDTO.LastName) is "" ? user.LastName : Capitalize(_updateUserDTO.LastName);
        user.Gender = Capitalize(_updateUserDTO.Gender) is "" ? user.Gender : Capitalize(_updateUserDTO.Gender);
        user.Email = string.IsNullOrWhiteSpace(_updateUserDTO.Email) ? user.Email : _updateUserDTO.Email.Trim();
        user.Bio = string.IsNullOrWhiteSpace(_updateUserDTO.Bio) ? user.Bio : _updateUserDTO.Bio;
        user.ImageUrl = string.IsNullOrWhiteSpace(_updateUserDTO.ImageUrl) ? user.ImageUrl : _updateUserDTO.ImageUrl;
        user.UpdateddAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task<List<UsersDTOs.UserProfileDTO>> SearchUser(string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return [];

        IQueryable<User> query = _context.Users;

        if (Guid.TryParse(searchQuery, out Guid id))
        {
            query = query.Where(u => u.Id == id);
        }
        else
        {
            var term = searchQuery.Trim().ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term));
        }

        return await query.Select(u => new UsersDTOs.UserProfileDTO
        {
            Id = u.Id,
            FirstName = u.FirstName,
            MiddleName = u.MiddleName,
            LastName = u.LastName,
            Gender = u.Gender,
            Email = u.Email,
            Bio = u.Bio,
            ImageUrl = u.ImageUrl,
            CreatedAt = u.CreatedAt,
        }).ToListAsync();
    }

    public async Task<bool> IsFollowing(Guid followerId, Guid followingId)
    {
        return await _context.Follows.AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }

    public async Task<bool> IsRequested(Guid senderId, Guid receiverId)
    {
        return await _context.FollowRequests.AnyAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId && fr.Status == FollowRequestStatus.Pending);
    }

    public async Task<bool> HasIncomingRequest(Guid receiverId, Guid senderId)
    {
        return await _context.FollowRequests.AnyAsync(fr => fr.ReceiverId == receiverId && fr.SenderId == senderId && fr.Status == FollowRequestStatus.Pending);
    }
}
