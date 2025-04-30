using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Comment;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers.Comment;

public class CreateCommentHandler : IRequestHandler<CreateCommentCmd, OperationResult<PostComment>>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public CreateCommentHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<PostComment>> Handle(CreateCommentCmd request, CancellationToken cancellationToken)
    {
        if (request.PostId == Guid.Empty)
        {
            return OperationResult<PostComment>.Failed("PostId cannot be null");
        }
        
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return OperationResult<PostComment>.Failed("Content cannot be null");
        }
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return OperationResult<PostComment>.Failed("Email cannot be null");
        }
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        if (user == null)
        {
            return OperationResult<PostComment>.Failed("User not found");
        }
        
        var comment = new PostComment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            Content = request.Content,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult<PostComment>.Succeeded(comment);
    }
}