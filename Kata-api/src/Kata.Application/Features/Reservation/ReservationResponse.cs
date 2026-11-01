namespace Kata.Application.Features.Reservation;

public sealed record ReservationResponse(
    Guid Id,
    Guid BookId,
    Guid ReaderId,
    string Status,
    DateTime ReservedAt,
    DateTime? BorrowedAt,
    DateTime? ReturnedAt)
{
    /// <summary>
    /// The reported status is the one observed at <paramref name="utcNow"/>, so a reservation
    /// left behind reads as Expired even though the stored row still says Reserved.
    /// </summary>
    public static ReservationResponse FromDomain(Domain.Reservation.Reservation reservation, DateTime utcNow) =>
        new(reservation.Id,
            reservation.BookId,
            reservation.ReaderId,
            reservation.StatusAt(utcNow).ToString(),
            reservation.ReservedAt,
            reservation.BorrowedAt,
            reservation.ReturnedAt);
}
