using SharedKernel;

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

    // Required by EF Core, which materialises entities without going through Create.
    private Reservation()
    {
    }

    private Reservation(Guid bookId, Guid readerId, DateTime reservedAt)
    {
        Id = Guid.NewGuid();
        BookId = bookId;
        ReaderId = readerId;
        Status = ReservationStatus.Reserved;
        ReservedAt = reservedAt;
    }

    public static Result<Reservation> Create(Guid bookId, Guid readerId, IDateTimeProvider dateTimeProvider)
    {
        if (bookId == Guid.Empty)
        {
            return Result.Failure<Reservation>(ReservationErrors.BookRequired);
        }

        if (readerId == Guid.Empty)
        {
            return Result.Failure<Reservation>(ReservationErrors.ReaderRequired);
        }

        return Result.Success(new Reservation(bookId, readerId, dateTimeProvider.UtcNow));
    }

    /// <summary>
    /// Picks the book up, which turns the reservation into an ongoing loan.
    /// </summary>
    public Result Borrow(IDateTimeProvider dateTimeProvider)
    {
        if (Status == ReservationStatus.Borrowed)
        {
            return Result.Failure(ReservationErrors.AlreadyBorrowed);
        }

        if (Status != ReservationStatus.Reserved)
        {
            return Result.Failure(ReservationErrors.NotReserved);
        }

        if (IsExpired(dateTimeProvider.UtcNow))
        {
            return Result.Failure(ReservationErrors.Expired);
        }

        Status = ReservationStatus.Borrowed;
        BorrowedAt = dateTimeProvider.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Gives up a reservation before the book has been picked up, which releases it.
    /// </summary>
    public Result Cancel()
    {
        if (Status != ReservationStatus.Reserved)
        {
            return Result.Failure(ReservationErrors.NotReserved);
        }

        Status = ReservationStatus.Cancelled;

        return Result.Success();
    }

    /// <summary>
    /// Brings a borrowed book back, which releases it.
    /// </summary>
    public Result Return(IDateTimeProvider dateTimeProvider)
    {
        if (Status != ReservationStatus.Borrowed)
        {
            return Result.Failure(ReservationErrors.NotBorrowed);
        }

        Status = ReservationStatus.Returned;
        ReturnedAt = dateTimeProvider.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Whether this reservation still holds its book, either awaiting pickup or on loan.
    /// This is what makes a book unavailable.
    /// </summary>
    public bool IsActive(DateTime utcNow) =>
        Status == ReservationStatus.Borrowed ||
        (Status == ReservationStatus.Reserved && !IsExpired(utcNow));

    /// <summary>
    /// The status as observed at <paramref name="utcNow"/>. Unlike <see cref="Status"/>, it reports
    /// an abandoned reservation as <see cref="ReservationStatus.Expired"/>. Never persist it.
    /// </summary>
    public ReservationStatus StatusAt(DateTime utcNow) =>
        IsExpired(utcNow) ? ReservationStatus.Expired : Status;

    private bool IsExpired(DateTime utcNow) =>
        Status == ReservationStatus.Reserved && utcNow >= ReservedAt + ExpiryWindow;
}
