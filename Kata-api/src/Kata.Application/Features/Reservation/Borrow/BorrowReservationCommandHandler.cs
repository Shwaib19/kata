using Kata.Domain.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Borrow;

internal sealed class BorrowReservationCommandHandler(
    IReservationReadRepository reservationReadRepository,
    IReservationWriteRepository reservationWriteRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<BorrowReservationCommand>
{
    public async Task<Result> Handle(BorrowReservationCommand command, CancellationToken cancellationToken)
    {
        Domain.Reservation.Reservation? reservation =
            await reservationReadRepository.GetById(command.ReservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure(ReservationErrors.NotFound);
        }

        // Borrow is where expiry is enforced: a reservation left untouched past its window
        // cannot be picked up, even though the stored status still reads Reserved.
        Result borrowed = reservation.Borrow(dateTimeProvider);

        if (borrowed.IsFailure)
        {
            return borrowed;
        }

        return await reservationWriteRepository.SaveChanges(cancellationToken);
    }
}
