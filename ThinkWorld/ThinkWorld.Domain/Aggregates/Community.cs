namespace ThinkWorld.Domain.Aggregates;

public record Community
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; } = string.Empty;
    public string? DeletedBy { get; init; }
    public DateTime? DeletedAt { get; init; }
}