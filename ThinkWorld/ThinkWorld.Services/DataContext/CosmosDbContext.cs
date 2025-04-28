using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;

namespace ThinkWorld.Services.DataContext;

public class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
    {
    }

    // Define DbSets for your entities
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityPost> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Community>().ToContainer("Communities").HasPartitionKey(c => c.Id).UseETagConcurrency();
        modelBuilder.Entity<CommunityPost>().ToContainer("Posts").HasPartitionKey(cp => cp.CommunityId).UseETagConcurrency();
    }
}