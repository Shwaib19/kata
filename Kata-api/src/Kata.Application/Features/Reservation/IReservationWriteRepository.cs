using SharedKernel;

namespace Kata.Application.Features.Reservation;

public interface IReservationWriteRepository
{
    Task Add(Domain.Reservation.Reservation reservation, CancellationToken cancellation = default);

    /// <summary>
    /// Settles the book's lapsed reservations by storing the status that reading them already
    /// derives. Reads do not need this; the uniqueness guarantee does, because it is built on
    /// the stored status and would otherwise keep a book held by a reservation nobody honoured.
    /// </summary>
    /// <param name="bookId">The book whose reservations are settled.</param>
    /// <param name="reservedBefore">The instant a reservation must predate to have lapsed.</param>
    Task ExpireLapsed(Guid bookId, DateTime reservedBefore, CancellationToken cancellation = default);

    /// <summary>
    /// Commits the pending changes. Returns a failure when the store refuses them, which is how
    /// two simultaneous reservations on the same book are told apart: the application cannot
    /// prevent that race, only the database can, and this is where its refusal becomes an error
    /// the use case understands.
    /// </summary>
    Task<Result> SaveChanges(CancellationToken cancellation = default);
}
