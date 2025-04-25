namespace ThinkWorld.Domain.Entities.Router;

public class RoutedComment
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required string PostId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required Guid RegionId { get; init; }
}