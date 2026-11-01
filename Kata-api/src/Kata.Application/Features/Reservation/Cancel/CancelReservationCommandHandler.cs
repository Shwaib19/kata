using Kata.Domain.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Cancel;

internal sealed class CancelReservationCommandHandler(
    IReservationReadRepository reservationReadRepository,
    IReservationWriteRepository reservationWriteRepository)
    : ICommandHandler<CancelReservationCommand>
{
    public async Task<Result> Handle(CancelReservationCommand command, CancellationToken cancellationToken)
    {
        Domain.Reservation.Reservation? reservation =
            await reservationReadRepository.GetById(command.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound);
        }

        // The domain decides whether the transition is legal; an already borrowed or returned
        // reservation is refused there, not here.
        Result cancelled = reservation.Cancel();

        if (cancelled.IsFailure)
        {
            return cancelled;
        }

        return await reservationWriteRepository.SaveChanges(cancellationToken);
    }
}
