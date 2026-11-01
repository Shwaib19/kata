using Kata.Application.Features.Book;
using Kata.Application.Features.Book.Create;
using Kata.Application.Features.Book.GetById;
using Kata.Application.Features.Book.Delete;
using Kata.Application.Features.Book.Search;
using Kata.Application.Features.Book.Update;
using Kata.Application.Features.Reader;
using Kata.Application.Features.Reader.Create;
using Kata.Application.Features.Reader.Delete;
using Kata.Application.Features.Reader.Update;
using Kata.Application.Features.Reservation;
using Kata.Application.Features.Reservation.GetById;
using ReaderGetById = Kata.Application.Features.Reader.GetById;
using Kata.Application.Features.Reservation.Reserve;
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
        services.AddScoped<ICommandHandler<UpdateBookCommand>, UpdateBookCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteBookCommand>, DeleteBookCommandHandler>();
        services.AddScoped<IQueryHandler<GetBookByIdQuery, BookResponse>, GetBookByIdQueryHandler>();
        services.AddScoped<IQueryHandler<SearchBooksQuery, IReadOnlyCollection<BookResponse>>,
            SearchBooksQueryHandler>();

        services.AddScoped<ICommandHandler<CreateReaderCommand, Guid>, CreateReaderCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateReaderCommand>, UpdateReaderCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteReaderCommand>, DeleteReaderCommandHandler>();
        services.AddScoped<IQueryHandler<ReaderGetById.GetReaderByIdQuery, ReaderResponse>,
            ReaderGetById.GetReaderByIdQueryHandler>();

        services.AddScoped<ICommandHandler<ReserveBookCommand, Guid>, ReserveBookCommandHandler>();
        services.AddScoped<IQueryHandler<GetReservationByIdQuery, ReservationResponse>, GetReservationByIdQueryHandler>();

        return services;
    }
}
