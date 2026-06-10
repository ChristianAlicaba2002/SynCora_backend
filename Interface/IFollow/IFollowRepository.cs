namespace syncora_server.Interface.IFollow;

public interface IFollowRepository
{
    Task SendFollowRequestAsync(Guid senderId, Guid receiverId);
    Task AcceptFollowRequestAsync(Guid requestId, Guid receiverId);
}