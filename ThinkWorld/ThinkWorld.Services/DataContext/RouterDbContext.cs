using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Entities.Router;

namespace ThinkWorld.Services.DataContext;

public class RouterDbContext : DbContext
{
    public RouterDbContext(DbContextOptions<RouterDbContext> options) : base(options)
    {
    }

    // Define DbSets for your entities
    public DbSet<RoutedComment> RoutedComments { get; set; }
    public DbSet<RoutedUser> Users { get; set; }
    public DbSet<RoutingRegion> Regions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RoutedComment>().ToContainer("Comments").HasPartitionKey(c => c.UserId);
        modelBuilder.Entity<RoutedUser>().ToContainer("Users").HasPartitionKey(cp => cp.Id);
        modelBuilder.Entity<RoutingRegion>().ToContainer("Regions").HasPartitionKey(cp => cp.Id);
    }
}