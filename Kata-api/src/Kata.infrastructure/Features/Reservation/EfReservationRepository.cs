using Kata.Application.Features.Reservation;
using Kata.Domain.Reservation;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Kata.infrastructure.Features.Reservation;

internal sealed class EfReservationRepository(KataDbContext dbContext)
    : IReservationReadRepository, IReservationWriteRepository
{
    public async Task Add(Domain.Reservation.Reservation reservation, CancellationToken cancellation = default) =>
        await dbContext.Reservations.AddAsync(reservation, cancellation);

    public async Task SaveChanges(CancellationToken cancellation = default) =>
        await dbContext.SaveChangesAsync(cancellation);

    public async Task<Domain.Reservation.Reservation?> GetById(Guid id, CancellationToken cancellation = default) =>
        await dbContext.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(reservation => reservation.Id == id, cancellation);

    public async Task<IReadOnlyCollection<Domain.Reservation.Reservation>> GetOpenByReader(
        Guid readerId, CancellationToken cancellation = default) =>
        await dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.ReaderId == readerId &&
                                  (reservation.Status == ReservationStatus.Reserved ||
                                   reservation.Status == ReservationStatus.Borrowed))
            .ToListAsync(cancellation);
}
