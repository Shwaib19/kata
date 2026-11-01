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
        ReservationDetails? details =
            await reservationReadRepository.GetDetailsById(query.ReservationId, cancellationToken);

        return details is null
            ? Result.Failure<ReservationResponse>(ReservationErrors.NotFound)
            : Result.Success(ReservationResponse.FromDomain(details, dateTimeProvider.UtcNow));
    }
}
