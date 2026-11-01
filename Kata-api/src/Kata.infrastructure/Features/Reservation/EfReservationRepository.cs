using Kata.Application.Features.Reservation;
using Kata.Domain.Reservation;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SharedKernel;

namespace Kata.infrastructure.Features.Reservation;

internal sealed class EfReservationRepository(KataDbContext dbContext)
    : IReservationReadRepository, IReservationWriteRepository
{
    /// <summary>
    /// The partial unique index that allows a book to appear once among the reservations
    /// still holding it. Its violation is the only conflict this table can report.
    /// </summary>
    private const string ActiveReservationPerBookIndex = "IX_Reservations_BookId";

    public async Task Add(Domain.Reservation.Reservation reservation, CancellationToken cancellation = default) =>
        await dbContext.Reservations.AddAsync(reservation, cancellation);

    public async Task ExpireLapsed(
        Guid bookId, DateTime reservedBefore, CancellationToken cancellation = default) =>
        await dbContext.Reservations
            .Where(reservation => reservation.BookId == bookId &&
                                  reservation.Status == ReservationStatus.Reserved &&
                                  reservation.ReservedAt <= reservedBefore)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    reservation => reservation.Status, ReservationStatus.Expired),
                cancellation);

    public async Task<Result> SaveChanges(CancellationToken cancellation = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellation);

            return Result.Success();
        }
        // Between the moment a use case finds a book free and the moment it inserts, another
        // request can have reserved it. Nothing in the application can close that window, so
        // the index arbitrates and the loser is told why. EF Core wraps the driver's error,
        // hence the test on the inner exception.
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
                  {
                      SqlState: PostgresErrorCodes.UniqueViolation,
                      ConstraintName: ActiveReservationPerBookIndex
                  })
        {
            return Result.Failure(ReservationErrors.BookAlreadyReserved);
        }
    }

    // Serves both the read and the write contract, and therefore tracks: a transition applied
    // by a use case has to be seen by SaveChanges.
    public async Task<Domain.Reservation.Reservation?> GetById(Guid id, CancellationToken cancellation = default) =>
        await dbContext.Reservations
            .FirstOrDefaultAsync(reservation => reservation.Id == id, cancellation);

    public async Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByReader(
        Guid readerId, CancellationToken cancellation = default) =>
        await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.ReaderId == readerId &&
                                  (reservation.Status == ReservationStatus.Reserved ||
                                   reservation.Status == ReservationStatus.Borrowed))
            .ToListAsync(cancellation);

    public async Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByBooks(
        IReadOnlyCollection<Guid> bookIds, CancellationToken cancellation = default) =>
        await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => bookIds.Contains(reservation.BookId) &&
                                  (reservation.Status == ReservationStatus.Reserved ||
                                   reservation.Status == ReservationStatus.Borrowed))
            .ToListAsync(cancellation);

    public async Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetAll(
        Guid? readerId, CancellationToken cancellation = default)
    {
        IQueryable<Domain.Reservation.Reservation> query = dbContext.Reservations.AsNoTracking();

        if (readerId is not null)
        {
            query = query.Where(reservation => reservation.ReaderId == readerId);
        }

        return await query
            .OrderByDescending(reservation => reservation.ReservedAt)
            .ToListAsync(cancellation);
    }
}
