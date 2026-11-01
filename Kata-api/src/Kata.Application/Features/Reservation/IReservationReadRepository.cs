namespace Kata.Application.Features.Reservation;

public interface IReservationReadRepository
{
    Task<Domain.Reservation.Reservation?> GetById(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// Returns the reader's reservations that are neither cancelled nor returned.
    /// Whether such a row still holds its book is for the domain to say, since a reservation
    /// left untouched past its window expires on its own: the caller filters with
    /// <see cref="Domain.Reservation.Reservation.IsActive"/> rather than trusting the stored status.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByReader(
        Guid readerId, CancellationToken cancellation = default);

    /// <summary>
    /// Returns the reservations of the given books that are neither cancelled nor returned.
    /// Whether a book is actually held is decided by the caller with
    /// <see cref="Domain.Reservation.Reservation.IsActive"/>, since availability is derived
    /// from time and never stored.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByBooks(
        IReadOnlyCollection<Guid> bookIds, CancellationToken cancellation = default);
}
