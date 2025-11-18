using UrlShortener.Domain;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UrlMapping> UrlMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        PopulateSeedData(modelBuilder);
    }

    private static void PopulateSeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UrlMapping>().HasData(new List<UrlMapping>
        {
            new()
            {
                Id = 1,
                OriginalUrl = "https://www.postgresql.org/download/windows/",
                ShortenedUrl = "abcdef"
            },
            new()
            {
                Id = 2,
                OriginalUrl = "https://www.postgresql.org/download/",
                ShortenedUrl = "123456"
            },
            new()
            {
                Id = 3,
                OriginalUrl = "https://www.postgresql.org/",
                ShortenedUrl = "xyz123"
            },
        });
    }
}