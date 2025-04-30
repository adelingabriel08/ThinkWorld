using MediatR;
using ThinkWorld.Domain.Events.Commands.Comment;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers.Comment;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCmd, OperationResult>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public DeleteCommentHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public Task<OperationResult> Handle(DeleteCommentCmd request, CancellationToken cancellationToken)
    {
        if (request.CommentId == Guid.Empty)
        {
            return Task.FromResult(OperationResult.Failed("CommentId cannot be null"));
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Task.FromResult(OperationResult.Failed("Email cannot be null"));
        }

        var userId = _userIdGenerator.ComputeUserId(request.Email);

        var comment = _context.Comments
            .FirstOrDefault(x => x.Id == request.CommentId && x.CreatedBy == userId);

        if (comment == null)
        {
            return Task.FromResult(OperationResult.Failed("Comment not found or not authorized to delete"));
        }

        _context.Comments.Remove(comment);
        _context.SaveChanges();

        return Task.FromResult(OperationResult.Succeeded());
    }
}