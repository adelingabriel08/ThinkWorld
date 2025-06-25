using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.PII.Router.Handlers;

public class GetRoutedCommentsByPostIdHandler : IRequestHandler<GetRoutedCommentsByPostIdCmd, OperationResult<List<RoutedComment>>>
{
    private readonly RouterDbContext _context;

    public GetRoutedCommentsByPostIdHandler(RouterDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<List<RoutedComment>>> Handle(GetRoutedCommentsByPostIdCmd request, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .Where(x => x.PostId == request.PostId)
            .ToListAsync(cancellationToken);
        return OperationResult<List<RoutedComment>>.Succeeded(comments);
    }
}
