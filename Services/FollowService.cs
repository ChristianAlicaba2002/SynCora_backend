using syncora_server.DTOs;
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

    public async Task<List<FollowRequestResponseDto>> GetUserFollowRequestAsync()
    {
        var userId = _currentUserService.UserId;
        if (userId is null) throw new Exception("User not found.");
        return await _followRepository.GetUserFollowRequestAsync(userId.Value);
    }

    public async Task SendFollowRequestAsync(Guid receiverId)
    {
        var senderId = _currentUserService.UserId;
        if (senderId is null) throw new Exception("User not found.");

        await _followRepository.SendFollowRequestAsync(senderId.Value, receiverId);
    }

    public async Task UnfollowAsync(Guid followingId)
    {
        var followerId = _currentUserService.UserId;
        if (followerId is null) throw new Exception("User not found.");

        await _followRepository.UnfollowAsync(followerId.Value, followingId);
    }

    public async Task CancelFollowRequestAsync(Guid receiverId)
    {
        var senderId = _currentUserService.UserId;
        if (senderId is null) throw new Exception("User not found.");

        await _followRepository.CancelFollowRequestAsync(senderId.Value, receiverId);
    }
}
