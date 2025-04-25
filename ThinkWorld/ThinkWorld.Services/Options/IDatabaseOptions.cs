using System.ComponentModel.DataAnnotations;

namespace ThinkWorld.Services.Options;

public abstract class DatabaseOptions
{
    [Required]
    public required string Endpoint { get; set; }
    
    [Required]
    public required string DatabaseName { get; set; }
    
    public string? EndpointKey { get; set; }
    
    public bool UseManagedIdentity { get; set; } = true;
}