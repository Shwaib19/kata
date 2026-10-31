using Kata.Application.Features.Book;
using Kata.infrastructure.Database;
using Kata.infrastructure.Features.Book;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Kata.infrastructure;

/// <summary>
/// Composition root for the infrastructure layer: persistence, external services and cross-cutting adapters.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the infrastructure layer services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration, used to resolve connection strings.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KataDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // One class backs both contracts: callers still depend on the narrow interface
        // they need, while the registration keeps a single EF Core implementation.
        services.AddScoped<EfBookRepository>();
        services.AddScoped<IBookReadRepository>(provider => provider.GetRequiredService<EfBookRepository>());
        services.AddScoped<IBookWriteRepository>(provider => provider.GetRequiredService<EfBookRepository>());

        return services;
    }
}
