using System;

namespace syncora_server.Models;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    // public List<Guid> Friends { get; set; } = [];
    public string ImageUrl {get;set;} = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateddAt { get; set; }
}
