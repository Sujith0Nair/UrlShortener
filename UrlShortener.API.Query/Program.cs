using Scalar.AspNetCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using UrlShortener.Application.Extensions;
using UrlShortener.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.API.Query;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        builder.Services.AddQueryServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
            app.MapOpenApi();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}