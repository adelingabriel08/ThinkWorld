using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Comment;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers.Comment;

public class GetPostCommentsHandler : IRequestHandler<GetPostCommentsCmd, OperationResult<List<PostComment>>>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public GetPostCommentsHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<List<PostComment>>> Handle(GetPostCommentsCmd request, CancellationToken cancellationToken)
    {
        if (request.PostId == Guid.Empty)
        {
            return OperationResult<List<PostComment>>.Failed("PostId cannot be null");
        }
        
        var comments = await _context.Comments
            .Where(x => x.PostId == request.PostId)
            .ToListAsync(cancellationToken);
        
        return OperationResult<List<PostComment>>.Succeeded(comments);
    }
}