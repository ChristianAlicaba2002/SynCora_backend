namespace syncora_server.Interface.IFollow;

public interface IFollowService
{
    Task SendFollowRequestAsync(Guid receiverId);
    Task AcceptFollowRequestAsync(Guid requestId);
}