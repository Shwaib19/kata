using Kata.Application.Features.Reservation.Reserve;
using Kata.Domain.Reservation;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.IntegrationTests;

/// <summary>
/// The bug the kata describes: between reading that a book is free and writing the reservation,
/// another request can do the same. Nothing in the application can close that gap, so the
/// database arbitrates. These two tests check that it does, and that the application reports it.
/// </summary>
public sealed class DoubleReservationTest(LibraryDatabase database) : IClassFixture<LibraryDatabase>
{
    [Fact]
    public async Task The_Database_Refuses_A_Second_Reservation_Holding_The_Same_Book()
    {
        await database.Reset();
        (Guid bookId, Guid firstReaderId, Guid secondReaderId) = await database.Seed();

        // Two scopes, therefore two DbContext instances and two connections. Sharing one would
        // share its change tracker, there would be a single save, and no conflict to observe.
        await using AsyncServiceScope firstScope = database.Services.CreateAsyncScope();
        await using AsyncServiceScope secondScope = database.Services.CreateAsyncScope();

        KataDbContext firstContext = firstScope.ServiceProvider.GetRequiredService<KataDbContext>();
        KataDbContext secondContext = secondScope.ServiceProvider.GetRequiredService<KataDbContext>();
        IDateTimeProvider clock = firstScope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        firstContext.Reservations.Add(Reservation.Create(bookId, firstReaderId, clock).Value);
        await firstContext.SaveChangesAsync();

        // Written straight through the context: the use case is bypassed on purpose, so that
        // what is proven here is the store's guarantee and nothing else.
        secondContext.Reservations.Add(Reservation.Create(bookId, secondReaderId, clock).Value);

        DbUpdateException refusal = await Assert.ThrowsAsync<DbUpdateException>(
            () => secondContext.SaveChangesAsync());

        PostgresException violation = Assert.IsType<PostgresException>(refusal.InnerException);

        Assert.Equal(PostgresErrorCodes.UniqueViolation, violation.SqlState);
        Assert.Equal(1, await database.CountActiveReservations(bookId));
    }

    [Fact]
    public async Task Two_Simultaneous_Requests_Leave_One_Reservation_And_One_Explicit_Refusal()
    {
        await database.Reset();
        (Guid bookId, Guid firstReaderId, Guid secondReaderId) = await database.Seed();

        // Started without awaiting the first, so both are in flight at once.
        Task<Result<Guid>> first = Reserve(bookId, firstReaderId);
        Task<Result<Guid>> second = Reserve(bookId, secondReaderId);

        Result<Guid>[] outcomes = await Task.WhenAll(first, second);

        Assert.Single(outcomes, outcome => outcome.IsSuccess);

        Result<Guid> refused = Assert.Single(outcomes, outcome => outcome.IsFailure);
        Assert.Equal(ReservationErrors.BookAlreadyReserved, refused.Error);

        // The assertion that matters: whatever the interleaving, the book is held once.
        Assert.Equal(1, await database.CountActiveReservations(bookId));
    }

    /// <summary>
    /// One request's worth of work: its own scope, as a web request would get.
    /// </summary>
    private async Task<Result<Guid>> Reserve(Guid bookId, Guid readerId)
    {
        await using AsyncServiceScope scope = database.Services.CreateAsyncScope();

        ICommandHandler<ReserveBookCommand, Guid> handler = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<ReserveBookCommand, Guid>>();

        return await handler.Handle(new ReserveBookCommand(bookId, readerId), CancellationToken.None);
    }
}
