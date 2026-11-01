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
        IReadOnlyCollection<ReservationDetails> reservations =
            await reservationReadRepository.GetAll(query.ReaderId, query.BookId, cancellationToken);

        DateTime utcNow = dateTimeProvider.UtcNow;

        return Result.Success<IReadOnlyCollection<ReservationResponse>>(
            reservations
                .Select(details => ReservationResponse.FromDomain(details, utcNow))
                .ToArray());
    }
}
