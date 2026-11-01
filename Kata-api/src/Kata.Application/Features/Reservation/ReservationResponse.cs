namespace Kata.Application.Features.Reservation;

public sealed record ReservationResponse(
    Guid Id,
    Guid BookId,
    string BookTitle,
    Guid ReaderId,
    string ReaderName,
    string Status,
    DateTime ReservedAt,
    DateTime? BorrowedAt,
    DateTime? ReturnedAt)
{
    /// <summary>
    /// The reported status is the one observed at <paramref name="utcNow"/>, so a reservation
    /// left behind reads as Expired even though the stored row still says Reserved.
    /// </summary>
    public static ReservationResponse FromDomain(ReservationDetails details, DateTime utcNow) =>
        new(details.Reservation.Id,
            details.Reservation.BookId,
            details.BookTitle,
            details.Reservation.ReaderId,
            details.ReaderName,
            details.Reservation.StatusAt(utcNow).ToString(),
            details.Reservation.ReservedAt,
            details.Reservation.BorrowedAt,
            details.Reservation.ReturnedAt);
}
