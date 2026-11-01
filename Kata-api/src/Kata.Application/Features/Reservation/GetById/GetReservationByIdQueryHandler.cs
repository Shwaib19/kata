using Kata.Domain.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.GetById;

internal sealed class GetReservationByIdQueryHandler(
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetReservationByIdQuery, ReservationResponse>
{
    public async Task<Result<ReservationResponse>> Handle(
        GetReservationByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.Reservation.Reservation? reservation =
            await reservationReadRepository.GetById(query.ReservationId, cancellationToken);

        return reservation is null
            ? Result.Failure<ReservationResponse>(ReservationErrors.NotFound)
            : Result.Success(ReservationResponse.FromDomain(reservation, dateTimeProvider.UtcNow));
    }
}
