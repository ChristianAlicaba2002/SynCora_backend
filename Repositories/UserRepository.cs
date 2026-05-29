using Microsoft.EntityFrameworkCore;
using syncora_server.Data;
using syncora_server.DTOs;
using syncora_server.Interface.IUser;
using syncora_server.Models;

namespace syncora_server.Repositories;

public class UserRepository(AppDbContext appDb) : IUserRepository
{
    private readonly AppDbContext _db = appDb;

    private static string Capitalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        value = value.Trim();
        return char.ToUpper(value[0]) + value.Substring(1).ToLower();
    }

    public async Task<User?> GetUserEmail(string email)
    {
        var userEmail = await _db.Users.FirstOrDefaultAsync(user => user.Email == email);

        return userEmail;
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

        _db.Users.Add(saveUser);
        await _db.SaveChangesAsync();
    }
}
