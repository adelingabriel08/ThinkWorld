namespace ThinkWorld.Domain.Entities.Router;

public record RoutingRegion
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string TopLevelDomain { get; init; }
}