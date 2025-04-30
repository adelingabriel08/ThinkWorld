using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Post;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers;

public class AddOrUpdatePostVoteHandler : IRequestHandler<AddOrUpdatePostVoteCmd, OperationResult<PostVote>>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public AddOrUpdatePostVoteHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<PostVote>> Handle(AddOrUpdatePostVoteCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return OperationResult<PostVote>.Failed("Email cannot be null");
        }
        
        if (request.PostId == Guid.Empty)
        {
            return OperationResult<PostVote>.Failed("PostId cannot be null");
        }
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user == null)
            return OperationResult<PostVote>.Failed("User not found");
        
        var postVote = await _context.PostVotes
            .FirstOrDefaultAsync(x => x.PostId == request.PostId && x.UserId == userId, cancellationToken);

        if (postVote == null)
        {
            if (request.Vote == null)
            {
                return OperationResult<PostVote>.Failed("Vote cannot be null");
            }
            
            postVote = new PostVote
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = userId,
                IsUpvote = VoteType.Upvote == request.Vote
            };
            
            await _context.PostVotes.AddAsync(postVote, cancellationToken);
        }
        else
        {
            if (request.Vote == null)
            {
                _context.Remove(postVote);
            }
            else
            {
                postVote.IsUpvote = request.Vote == VoteType.Upvote;

            }
        }
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult<PostVote>.Succeeded(postVote);
    }
}