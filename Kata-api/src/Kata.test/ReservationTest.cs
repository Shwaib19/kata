using Kata.Domain.Reservation;
using SharedKernel;

namespace Kata.test;

public class ReservationTest
{
    private static readonly DateTime ReservedAt = new(2026, 10, 31, 12, 0, 0, DateTimeKind.Utc);

    private readonly Guid _bookId = Guid.NewGuid();
    private readonly Guid _readerId = Guid.NewGuid();
    private readonly FakeDateTimeProvider _dateTimeProvider = new(ReservedAt);

    private Reservation AReservation()
    {
        Result<Reservation> result = Reservation.Create(_bookId, _readerId, _dateTimeProvider);

        Assert.True(result.IsSuccess);

        return result.Value;
    }

    [Fact]
    public void Create_Should_Reserve_The_Book_For_The_Reader()
    {
        Result<Reservation> result = Reservation.Create(_bookId, _readerId, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(_bookId, result.Value.BookId);
        Assert.Equal(_readerId, result.Value.ReaderId);
        Assert.Equal(ReservationStatus.Reserved, result.Value.Status);
        Assert.Equal(ReservedAt, result.Value.ReservedAt);
    }

    [Fact]
    public void Create_Should_Fail_When_The_Book_Is_Missing()
    {
        Result<Reservation> result = Reservation.Create(Guid.Empty, _readerId, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.BookRequired, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_When_The_Reader_Is_Missing()
    {
        Result<Reservation> result = Reservation.Create(_bookId, Guid.Empty, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.ReaderRequired, result.Error);
    }

    [Fact]
    public void Borrow_Should_Move_A_Reserved_Reservation_To_Borrowed()
    {
        Reservation reservation = AReservation();
        _dateTimeProvider.Advance(TimeSpan.FromHours(2));

        Result result = reservation.Borrow(_dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Borrowed, reservation.Status);
        Assert.Equal(_dateTimeProvider.UtcNow, reservation.BorrowedAt);
    }

    [Fact]
    public void Borrow_Should_Fail_When_The_Reservation_Is_Already_Borrowed()
    {
        Reservation reservation = AReservation();
        reservation.Borrow(_dateTimeProvider);

        Result result = reservation.Borrow(_dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.AlreadyBorrowed, result.Error);
    }

    [Fact]
    public void Borrow_Should_Fail_When_The_Reservation_Is_Cancelled()
    {
        Reservation reservation = AReservation();
        reservation.Cancel();

        Result result = reservation.Borrow(_dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.NotReserved, result.Error);
    }

    [Fact]
    public void Borrow_Should_Fail_When_The_Reservation_Has_Expired()
    {
        Reservation reservation = AReservation();
        _dateTimeProvider.Advance(Reservation.ExpiryWindow);

        Result result = reservation.Borrow(_dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.Expired, result.Error);
    }

    [Fact]
    public void Cancel_Should_Move_A_Reserved_Reservation_To_Cancelled()
    {
        Reservation reservation = AReservation();

        Result result = reservation.Cancel();

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void Cancel_Should_Fail_When_The_Book_Is_Already_Borrowed()
    {
        Reservation reservation = AReservation();
        reservation.Borrow(_dateTimeProvider);

        Result result = reservation.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.NotReserved, result.Error);
    }

    [Fact]
    public void Cancel_Should_Fail_When_The_Reservation_Is_Already_Cancelled()
    {
        Reservation reservation = AReservation();
        reservation.Cancel();

        Result result = reservation.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.NotReserved, result.Error);
    }

    [Fact]
    public void Return_Should_Move_A_Borrowed_Reservation_To_Returned()
    {
        Reservation reservation = AReservation();
        reservation.Borrow(_dateTimeProvider);
        _dateTimeProvider.Advance(TimeSpan.FromDays(7));

        Result result = reservation.Return(_dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReservationStatus.Returned, reservation.Status);
        Assert.Equal(_dateTimeProvider.UtcNow, reservation.ReturnedAt);
    }

    [Fact]
    public void Return_Should_Fail_When_The_Book_Was_Never_Borrowed()
    {
        Reservation reservation = AReservation();

        Result result = reservation.Return(_dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(ReservationErrors.NotBorrowed, result.Error);
    }

    [Fact]
    public void IsActive_Should_Hold_The_Book_Until_The_Reservation_Expires()
    {
        Reservation reservation = AReservation();

        Assert.True(reservation.IsActive(_dateTimeProvider.UtcNow));

        _dateTimeProvider.Advance(Reservation.ExpiryWindow);

        Assert.False(reservation.IsActive(_dateTimeProvider.UtcNow));
    }

    [Fact]
    public void IsActive_Should_Be_False_Once_The_Reservation_Is_Cancelled()
    {
        Reservation reservation = AReservation();
        reservation.Cancel();

        Assert.False(reservation.IsActive(_dateTimeProvider.UtcNow));
    }

    [Fact]
    public void IsActive_Should_Be_False_Once_The_Book_Is_Returned()
    {
        Reservation reservation = AReservation();
        reservation.Borrow(_dateTimeProvider);
        reservation.Return(_dateTimeProvider);

        Assert.False(reservation.IsActive(_dateTimeProvider.UtcNow));
    }

    [Fact]
    public void StatusAt_Should_Not_Be_Expired_Just_Before_The_Window_Elapses()
    {
        Reservation reservation = AReservation();
        _dateTimeProvider.Advance(Reservation.ExpiryWindow - TimeSpan.FromMinutes(1));

        Assert.Equal(ReservationStatus.Reserved, reservation.StatusAt(_dateTimeProvider.UtcNow));
    }

    [Fact]
    public void StatusAt_Should_Be_Expired_Once_The_Window_Has_Elapsed()
    {
        Reservation reservation = AReservation();
        _dateTimeProvider.Advance(Reservation.ExpiryWindow);

        Assert.Equal(ReservationStatus.Expired, reservation.StatusAt(_dateTimeProvider.UtcNow));
        Assert.Equal(ReservationStatus.Reserved, reservation.Status);
    }

    [Fact]
    public void StatusAt_Should_Stay_Cancelled_Even_After_The_Window_Has_Elapsed()
    {
        Reservation reservation = AReservation();
        reservation.Cancel();
        _dateTimeProvider.Advance(Reservation.ExpiryWindow);

        Assert.Equal(ReservationStatus.Cancelled, reservation.StatusAt(_dateTimeProvider.UtcNow));
    }

    [Fact]
    public void StatusAt_Should_Stay_Borrowed_Even_After_The_Window_Has_Elapsed()
    {
        Reservation reservation = AReservation();
        reservation.Borrow(_dateTimeProvider);
        _dateTimeProvider.Advance(Reservation.ExpiryWindow);

        Assert.Equal(ReservationStatus.Borrowed, reservation.StatusAt(_dateTimeProvider.UtcNow));
    }
}
