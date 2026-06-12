using syncora_server.DTOs;

namespace syncora_server.Interface.IFollow;

public interface IFollowService
{
    Task<List<FollowRequestResponseDto>> GetUserFollowRequestAsync();
    Task SendFollowRequestAsync(Guid receiverId);
    Task AcceptFollowRequestAsync(Guid requestId);
}