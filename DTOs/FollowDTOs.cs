namespace syncora_server.DTOs;

public class AcceptFollowRequestDto
{
    public Guid RequestId { get; set; }
}

public class FollowRequestSenderDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class FollowRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public FollowRequestSenderDto Sender { get; set; } = null!;
}

public class UnfollowDto
{
    public Guid ReceiverId { get; set; }
}

public class CancelFollowRequestDto
{
    public Guid FolloweeId { get; set; }
}

