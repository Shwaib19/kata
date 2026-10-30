using SharedKernel;

namespace Kata.Domain.Rerservation;

public class ReservationErrors
{
    public static Error NotFound => new("Reservation.NotFound", "Reservation not found.", ErrorType.NotFound);
}