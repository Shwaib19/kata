using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.List;

internal sealed class ListReservationsQueryHandler(
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<ListReservationsQuery, IReadOnlyCollection<ReservationResponse>>
{
    public async Task<Result<IReadOnlyCollection<ReservationResponse>>> Handle(
        ListReservationsQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Domain.Reservation.Reservation> reservations =
            await reservationReadRepository.GetAll(query.ReaderId, cancellationToken);

        DateTime utcNow = dateTimeProvider.UtcNow;

        return Result.Success<IReadOnlyCollection<ReservationResponse>>(
            reservations
                .Select(reservation => ReservationResponse.FromDomain(reservation, utcNow))
                .ToArray());
    }
}
