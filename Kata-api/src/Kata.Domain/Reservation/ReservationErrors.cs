using SharedKernel;

namespace Kata.Domain.Reservation;

public static class ReservationErrors
{
    public static Error NotFound => new(
        "Reservation.NotFound",
        "Reservation not found.",
        ErrorType.NotFound);

    public static Error BookRequired => new(
        "Reservation.BookRequired",
        "A reservation requires a book.",
        ErrorType.Validation);

    public static Error ReaderRequired => new(
        "Reservation.ReaderRequired",
        "A reservation requires a reader.",
        ErrorType.Validation);

    public static Error AlreadyBorrowed => new(
        "Reservation.AlreadyBorrowed",
        "This reservation has already been borrowed.",
        ErrorType.Conflict);

    public static Error NotReserved => new(
        "Reservation.NotReserved",
        "Only a reservation still awaiting pickup can be borrowed or cancelled.",
        ErrorType.Conflict);

    public static Error Expired => new(
        "Reservation.Expired",
        "This reservation has expired and no longer holds the book.",
        ErrorType.Conflict);

    public static Error NotBorrowed => new(
        "Reservation.NotBorrowed",
        "Only a borrowed book can be returned.",
        ErrorType.Conflict);

    public static Error QuotaExceeded => new(
        "Reservation.QuotaExceeded",
        $"A reader cannot hold more than {Reservation.MaxActivePerReader} reservations at once.",
        ErrorType.Conflict);

    public static Error BookAlreadyReserved => new(
        "Reservation.BookAlreadyReserved",
        "This book is already reserved or borrowed.",
        ErrorType.Conflict);
}
