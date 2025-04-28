namespace ThinkWorld.Domain.Entities.Router;

public class RoutedUser
{
    public required string Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required Guid RegionId { get; set; }
}