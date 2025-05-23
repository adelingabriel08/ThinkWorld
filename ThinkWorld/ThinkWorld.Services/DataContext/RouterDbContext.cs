﻿using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Entities.Router;

namespace ThinkWorld.Services.DataContext;

public class RouterDbContext : DbContext
{
    public RouterDbContext(DbContextOptions<RouterDbContext> options) : base(options)
    {
    }
    
    public DbSet<RoutedComment> Comments { get; set; }
    public DbSet<RoutedUser> Users { get; set; }
    public DbSet<RoutingRegion> Regions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RoutedComment>().ToContainer("Comments").HasPartitionKey(c => c.UserId).UseETagConcurrency();
        modelBuilder.Entity<RoutedUser>().ToContainer("Users").HasPartitionKey(cp => cp.Id).UseETagConcurrency();
        modelBuilder.Entity<RoutingRegion>().ToContainer("Regions").HasPartitionKey(cp => cp.Id).UseETagConcurrency();
    }
    public static IEnumerable<string> GetTableNames()
    {
        return new List<string>
        {
            "Comments",
            "Users",
            "Regions"
        };
    }
    
    public async Task SeedRegionsAsync(CancellationToken cancellationToken = default)
    {
        var region = await Regions
            .FirstOrDefaultAsync(r => r.Name == "UK South", cancellationToken);
        if (region != null)
        {
            // region already exists
            return;
        }

        region = new RoutingRegion
        {
            Id = Guid.NewGuid(),
            Name = "UK South",
            TopLevelDomain = "https://localhost:1234"
        };

        await Regions.AddAsync(region, cancellationToken);
        await SaveChangesAsync(cancellationToken);
    }
}