namespace ThinkWorld.Domain.Aggregates;

public record PostComment
{
    public Guid PostId { get; set; }
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}