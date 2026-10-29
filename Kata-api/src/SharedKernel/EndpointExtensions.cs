using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel;

/// <summary>
/// Discovers and registers <see cref="IEndpoint"/> implementations so that endpoints live
/// next to their feature instead of being declared in a single startup file.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Registers every non-abstract <see cref="IEndpoint"/> found in the given assembly.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assembly">The assembly to scan for endpoints.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] descriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerableRange(descriptors);

        return services;
    }

    /// <summary>
    /// Maps every registered <see cref="IEndpoint"/> onto the application's route table.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="routeGroup">An optional group to nest the endpoints under (e.g. a versioned prefix).</param>
    /// <returns>The same application, for chaining.</returns>
    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroup = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroup is null ? app : routeGroup;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    private static void TryAddEnumerableRange(this IServiceCollection services, ServiceDescriptor[] descriptors)
    {
        foreach (ServiceDescriptor descriptor in descriptors)
        {
            if (!services.Any(existing => existing.ServiceType == descriptor.ServiceType &&
                                          existing.ImplementationType == descriptor.ImplementationType))
            {
                services.Add(descriptor);
            }
        }
    }
}
