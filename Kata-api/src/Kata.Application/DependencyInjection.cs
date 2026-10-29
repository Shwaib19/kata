using Microsoft.Extensions.DependencyInjection;

namespace Kata.Application;

/// <summary>
/// Composition root for the application layer: command/query handlers, validators and pipeline behaviours.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        
        return services;
    }
}
