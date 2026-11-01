using Kata.Domain.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Return;

internal sealed class ReturnReservationCommandHandler(
    IReservationReadRepository reservationReadRepository,
    IReservationWriteRepository reservationWriteRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ReturnReservationCommand>
{
    public async Task<Result> Handle(ReturnReservationCommand command, CancellationToken cancellationToken)
    {
        Domain.Reservation.Reservation? reservation =
            await reservationReadRepository.GetById(command.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound);
        }

        Result returned = reservation.Return(dateTimeProvider);

        if (returned.IsFailure)
        {
            return returned;
        }

        return await reservationWriteRepository.SaveChanges(cancellationToken);
    }
}
