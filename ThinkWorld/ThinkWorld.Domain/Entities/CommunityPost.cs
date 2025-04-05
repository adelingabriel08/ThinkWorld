namespace ThinkWorld.Domain.Entities;

public record CommunityPost
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string UpdatedBy { get; init; } = string.Empty;
    public string? DeletedBy { get; init; }
    public DateTime? DeletedAt { get; init; }
    public List<PostComment> Comments { get; init; } = new();
}