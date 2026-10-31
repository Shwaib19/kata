namespace Kata.Domain.Reservation;

public enum ReservationStatus
{
    Reserved = 0,
    Borrowed = 1,
    Returned = 2,
    Cancelled = 3,
    Expired = 4,
}
