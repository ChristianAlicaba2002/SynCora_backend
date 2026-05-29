namespace syncora_server.Models;

public class Activity
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonIgnore]
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
}
