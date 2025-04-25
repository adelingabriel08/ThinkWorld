namespace ThinkWorld.Domain.Entities.Router;

public record RoutedUser
{
    public required string Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid RegionId { get; init; }
}