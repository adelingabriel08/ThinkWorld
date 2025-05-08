using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;

namespace ThinkWorld.Services.DataContext;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    // Define DbSets for your entities
    public DbSet<PostComment> Comments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PostVote> PostVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<PostComment>().ToContainer("Comments").HasPartitionKey(c => c.PostId).UseETagConcurrency();
        modelBuilder.Entity<User>().ToContainer("Users").HasPartitionKey(cp => cp.Id).UseETagConcurrency();
        modelBuilder.Entity<PostVote>().ToContainer("PostVotes").HasPartitionKey(cp => cp.PostId).UseETagConcurrency();
    }
    
    public static IEnumerable<string> GetTableNames()
    {
        return new List<string>
        {
            "Comments",
            "Users",
            "PostVotes"
        };
    }
}