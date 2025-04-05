namespace ThinkWorld.Domain.Entities;

public record PostVote
{
    public Guid PostId { get; init; }
    public Guid UserId { get; init; }
    public bool IsUpvote { get; init; }
}