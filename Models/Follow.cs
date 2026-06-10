namespace syncora_server.Models;

public class Follow
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public User Follower { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public User Following { get; set; } = null!;
}