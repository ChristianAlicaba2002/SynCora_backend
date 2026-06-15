namespace syncora_server.DTOs;

public class UsersDTOs
{
    public class RegisterUserDTOs
    {
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Gender { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginUserDTOs
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UserProfileDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsRequested { get; set; }
        public bool HasIncomingRequest { get; set; }
    }

    public class UpdateUserDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}