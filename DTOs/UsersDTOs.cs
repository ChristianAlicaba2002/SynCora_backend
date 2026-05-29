using System;

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

}