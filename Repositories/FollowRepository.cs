using Microsoft.EntityFrameworkCore;
using syncora_server.Data;
using syncora_server.DTOs;
using syncora_server.Interface.IFollow;
using syncora_server.Models;

namespace syncora_server.Repositories;

public class FollowRepository(AppDbContext context) : IFollowRepository
{
    private readonly AppDbContext _context = context;

    public async Task AcceptFollowRequestAsync(Guid requestId, Guid receiverId)
    {
        if (requestId == Guid.Empty)
            throw new Exception("Invalid follow request id.");

        var followRequest = await _context.FollowRequests.FindAsync(requestId);

        if (followRequest is null)
            throw new Exception("Follow request not found.");

        if (followRequest.ReceiverId != receiverId)
            throw new UnauthorizedAccessException("You are not authorized to accept this follow request.");

        if (followRequest.Status != FollowRequestStatus.Pending)
            throw new Exception("This follow request is no longer pending.");

        var alreadyFollowing = await _context.Follows.AnyAsync(f =>
            f.FollowerId == followRequest.SenderId && f.FollowingId == followRequest.ReceiverId);

        if (alreadyFollowing)
            throw new Exception("This user is already following you.");

        followRequest.Status = FollowRequestStatus.Accepted;

        await _context.Follows.AddAsync(new Follow
        {
            Id = Guid.NewGuid(),
            FollowerId = followRequest.SenderId,
            FollowingId = followRequest.ReceiverId,
            CreatedAt = DateTime.UtcNow,
        });

        _context.FollowRequests.Remove(followRequest);

        await _context.SaveChangesAsync();
    }

    public async Task<List<FollowRequestResponseDto>> GetUserFollowRequestAsync(Guid userId)
    {
        return await _context.FollowRequests
            .Where(fr => fr.ReceiverId == userId && fr.Status == FollowRequestStatus.Pending)
            .Select(fr => new FollowRequestResponseDto
            {
                Id = fr.Id,
                SenderId = fr.SenderId,
                ReceiverId = fr.ReceiverId,
                Status = fr.Status.ToString(),
                CreatedAt = fr.CreatedAt,
                Sender = new FollowRequestSenderDto
                {
                    Id = fr.Sender.Id,
                    FirstName = fr.Sender.FirstName,
                    MiddleName = fr.Sender.MiddleName,
                    LastName = fr.Sender.LastName,
                    ImageUrl = fr.Sender.ImageUrl,
                },
            })
            .ToListAsync();
    }

    public async Task SendFollowRequestAsync(Guid senderId, Guid receiverId)
    {
        if (senderId == receiverId)
            throw new Exception("You cannot follow yourself.");

        var receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
        if (!receiverExists)
            throw new Exception("User not found.");

        var alreadyFollowing = await _context.Follows.AnyAsync(f =>
            f.FollowerId == senderId && f.FollowingId == receiverId);

        if (alreadyFollowing)
            throw new Exception("You are already following this user.");

        var pendingRequest = await _context.FollowRequests.AnyAsync(fr =>
            fr.SenderId == senderId &&
            fr.ReceiverId == receiverId &&
            fr.Status == FollowRequestStatus.Pending);

        if (pendingRequest)
            throw new Exception("A follow request is already pending.");

        var followRequest = new FollowRequest
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Status = FollowRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };

        await _context.FollowRequests.AddAsync(followRequest);
        await _context.SaveChangesAsync();
    }

    public async Task UnfollowAsync(Guid followerId, Guid followingId)
    {
        var follow = await _context.Follows.FirstOrDefaultAsync(f =>
            f.FollowerId == followerId && f.FollowingId == followingId);

        if (follow is not null)
        {
            _context.Follows.Remove(follow);
        }

        var followRequest = await _context.FollowRequests.FirstOrDefaultAsync(fr =>
            fr.SenderId == followerId && fr.ReceiverId == followingId);

        if (followRequest is not null)
        {
            _context.FollowRequests.Remove(followRequest);
        }

        await _context.SaveChangesAsync();
    }

    public async Task CancelFollowRequestAsync(Guid senderId, Guid receiverId)
    {
        var followRequest = await _context.FollowRequests.FirstOrDefaultAsync(fr =>
            fr.SenderId == senderId &&
            fr.ReceiverId == receiverId &&
            fr.Status == FollowRequestStatus.Pending);

        if (followRequest is not null)
        {
            _context.FollowRequests.Remove(followRequest);
            await _context.SaveChangesAsync();
        }
    }
}
