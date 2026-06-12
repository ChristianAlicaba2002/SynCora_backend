using syncora_server.DTOs;

namespace syncora_server.Interface.IFollow;

public interface IFollowRepository
{
    Task<List<FollowRequestResponseDto>> GetUserFollowRequestAsync(Guid userId);
    Task SendFollowRequestAsync(Guid senderId, Guid receiverId);
    Task AcceptFollowRequestAsync(Guid requestId, Guid receiverId);
}