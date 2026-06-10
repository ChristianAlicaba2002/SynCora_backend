using syncora_server.Interface.ICurrentUser;
using syncora_server.Interface.IFollow;

namespace syncora_server.Services;

public class FollowService(IFollowRepository followRepository, ICurrentUserService currentUserService) : IFollowService
{
    private readonly IFollowRepository _followRepository = followRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    public async Task AcceptFollowRequestAsync(Guid requestId)
    {
        var receiverId = _currentUserService.UserId;
        if (receiverId is null) throw new Exception("User not found.");

        await _followRepository.AcceptFollowRequestAsync(requestId, receiverId.Value);
    }

    public async Task SendFollowRequestAsync(Guid receiverId)
    {
        var senderId = _currentUserService.UserId;
        if (senderId is null) throw new Exception("User not found.");

        await _followRepository.SendFollowRequestAsync(senderId.Value, receiverId);
    }
}
