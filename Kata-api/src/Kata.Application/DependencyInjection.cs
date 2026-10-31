using Kata.Application.Features.Book;
using Kata.Application.Features.Book.Create;
using Kata.Application.Features.Book.GetById;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Messaging;

namespace Kata.Application;

/// <summary>
/// Composition root for the application layer: command/query handlers, validators and pipeline behaviours.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer services.
    /// Handlers are listed explicitly rather than discovered by reflection: the list is short,
    /// and a missing registration then fails at compile time instead of on the first request.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateBookCommand, Guid>, CreateBookCommandHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookResponse>, GetBookByIdQueryHandler>();

        return services;
    }
}
