using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;

namespace ThinkWorld.Services.DataContext;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
    {
    }

    // Define DbSets for your entities
    public DbSet<PostComment> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PostVote> PostVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<PostComment>().ToContainer("Comments").HasPartitionKey(c => c.CreatedBy);
        modelBuilder.Entity<User>().ToContainer("Users").HasPartitionKey(cp => cp.Id);
        modelBuilder.Entity<PostVote>().ToContainer("PostVotes").HasPartitionKey(cp => cp.UserId);
    }
}