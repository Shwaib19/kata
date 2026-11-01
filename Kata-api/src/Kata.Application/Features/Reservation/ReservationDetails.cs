namespace Kata.Application.Features.Reservation;

/// <summary>
/// A reservation together with the names a screen has to show for it.
///
/// The reservation itself is carried as the entity rather than flattened, because the status a
/// reader sees is derived from the clock by the domain, and only the entity can compute it.
/// </summary>
public sealed record ReservationDetails(
    Domain.Reservation.Reservation Reservation,
    string BookTitle,
    string ReaderName);
