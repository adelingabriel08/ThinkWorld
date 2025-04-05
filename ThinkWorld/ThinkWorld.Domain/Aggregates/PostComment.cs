namespace ThinkWorld.Domain.Aggregates;

public record PostComment
{
    public Guid PostId { get; init; }
    public Guid Id { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
}