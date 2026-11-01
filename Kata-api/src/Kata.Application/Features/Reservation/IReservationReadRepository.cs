namespace Kata.Application.Features.Reservation;

public interface IReservationReadRepository
{
    /// <summary>
    /// The reservation as the domain knows it, for a use case about to apply a transition.
    /// </summary>
    Task<Domain.Reservation.Reservation?> GetById(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// The same reservation with the names a screen shows, for a query rather than a transition.
    /// </summary>
    Task<ReservationDetails?> GetDetailsById(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// Returns the reader's reservations that are neither cancelled nor returned.
    /// Whether such a row still holds its book is for the domain to say, since a reservation
    /// left untouched past its window expires on its own: the caller filters with
    /// <see cref="Domain.Reservation.Reservation.IsActive"/> rather than trusting the stored status.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByReader(
        Guid readerId, CancellationToken cancellation = default);

    /// <summary>
    /// The same as <see cref="GetOpenByReader"/> with the names, for the reader's own page.
    /// Kept apart so that enforcing the quota does not pay for a join it has no use for.
    /// </summary>
    Task<IReadOnlyCollection<ReservationDetails>> GetOpenDetailsByReader(
        Guid readerId, CancellationToken cancellation = default);

    /// <summary>
    /// Returns the reservations of the given books that are neither cancelled nor returned.
    /// Whether a book is actually held is decided by the caller with
    /// <see cref="Domain.Reservation.Reservation.IsActive"/>, since availability is derived
    /// from time and never stored.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByBooks(
        IReadOnlyCollection<Guid> bookIds, CancellationToken cancellation = default);

    /// <summary>
    /// The same as <see cref="GetOpenByReader"/> for several readers at once, so a list of
    /// readers can report how many books each one holds without one query per row.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByReaders(
        IReadOnlyCollection<Guid> readerIds, CancellationToken cancellation = default);

    /// <summary>
    /// Every reservation ever made, most recent first. Each criterion is optional and they
    /// combine, so a book and a reader together give that pair's history.
    /// </summary>
    Task<IReadOnlyCollection<ReservationDetails>> GetAll(
        Guid? readerId, Guid? bookId, CancellationToken cancellation = default);
}
