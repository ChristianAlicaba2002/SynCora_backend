namespace syncora_server.Models;

public enum FollowRequestStatus
{
    Pending,
    Accepted,
    Rejected
}

public class FollowRequest
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public FollowRequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public User Sender { get; set; } = null!;
    [System.Text.Json.Serialization.JsonIgnore]
    public User Receiver { get; set; } = null!;
}