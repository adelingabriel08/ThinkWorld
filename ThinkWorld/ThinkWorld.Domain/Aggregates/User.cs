namespace ThinkWorld.Domain.Aggregates;

public record User
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }
    public bool Annonymised { get; init; } = false;
    public DateTime? AnnonymisedAt { get; init; }
    public List<Community> JoinedCommunities { get; init; } = new();
}