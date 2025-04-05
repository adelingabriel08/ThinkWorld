namespace ThinkWorld.Domain.Entities;

public record User
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? ImageUrl { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool Annonymised { get; init; } = false;
    public DateTime? AnnonymisedAt { get; init; }
    public List<Community> JoinedCommunities { get; init; } = new();
}