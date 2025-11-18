using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

namespace UrlShortener.Infrastructure;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetRootEndPointPath())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.development.json")
            .AddUserSecrets<ApplicationDbContextFactory>()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string GetRootEndPointPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var rootEndPointPath = Path.Combine(currentDirectory, "..", "UrlShortener.API.Command");
        return rootEndPointPath;
    }
}