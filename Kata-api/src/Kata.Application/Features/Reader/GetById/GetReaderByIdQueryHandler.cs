using Kata.Application.Features.Reservation;
using Kata.Domain.Reader;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.GetById;

internal sealed class GetReaderByIdQueryHandler(
    IReaderReadRepository readerReadRepository,
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetReaderByIdQuery, ReaderResponse>
{
    public async Task<Result<ReaderResponse>> Handle(
        GetReaderByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.Reader.Reader? reader = await readerReadRepository.GetById(query.ReaderId, cancellationToken);

        if (reader is null)
        {
            return Result.Failure<ReaderResponse>(ReaderErrors.NotFound);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        // "Current" excludes the reservations that lapsed without being picked up, which the
        // stored status cannot tell apart from the live ones.
        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByReader(query.ReaderId, cancellationToken);

        return Result.Success(
            ReaderResponse.FromDomain(
                reader,
                open.Where(reservation => reservation.IsActive(utcNow)),
                utcNow));
    }
}
