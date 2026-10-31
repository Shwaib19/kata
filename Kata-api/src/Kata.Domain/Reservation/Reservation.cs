namespace Kata.Domain.Reservation;

public class Reservation
{
    public static readonly TimeSpan ExpiryWindow = TimeSpan.FromHours(48);

    public Guid Id { get; init; }
    public Guid BookId { get; init; }
    public Guid ReaderId { get; init; }
    public ReservationStatus Status { get; private set; }
    public DateTime ReservedAt { get; init; }
    public DateTime? BorrowedAt { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
}
