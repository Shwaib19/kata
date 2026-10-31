using Kata.infrastructure.Database;
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

        return services;
    }
}
