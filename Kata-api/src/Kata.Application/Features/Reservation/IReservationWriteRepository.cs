namespace Kata.Application.Features.Reservation;

public interface IReservationWriteRepository
{
    Task Add(Domain.Reservation.Reservation reservation, CancellationToken cancellation = default);

    Task SaveChanges(CancellationToken cancellation = default);
}
