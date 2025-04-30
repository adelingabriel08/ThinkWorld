namespace ThinkWorld.Domain.Aggregates;

public record PostVote
{
    public Guid Id { get; init; }
    public Guid PostId { get; init; }
    public string UserId { get; init; }
    public bool IsUpvote { get; set; }
}