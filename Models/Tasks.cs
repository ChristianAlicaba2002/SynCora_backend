namespace syncora_server.Models;

public class Tasks
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Pending | In Progess | Done
    public string Priority { get; set; } = string.Empty; // High | Medium | Low
    public DateTime CreatedAt { get; set; }
    public DateTime DueDate { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public User? User { get; set; }
}
