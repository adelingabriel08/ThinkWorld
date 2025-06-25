using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.PII.Router.Handlers;

public class CreateRoutedCommentHandler : IRequestHandler<CreateRoutedCommentCmd, OperationResult<RoutedComment>>
{
    private readonly RouterDbContext _context;

    public CreateRoutedCommentHandler(RouterDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<RoutedComment>> Handle(CreateRoutedCommentCmd request, CancellationToken cancellationToken)
    {
        var comment = new RoutedComment
        {
            Id = request.Id,
            UserId = request.UserId,
            PostId = request.PostId,
            RegionId = request.RegionId,
            CreatedAt = request.CreatedAt
        };
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult<RoutedComment>.Succeeded(comment);
    }
}
