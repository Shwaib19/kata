using Kata.Application.Features.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Search;

internal sealed class SearchReadersQueryHandler(
    IReaderReadRepository readerReadRepository,
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<SearchReadersQuery, IReadOnlyCollection<ReaderSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<ReaderSummaryResponse>>> Handle(
        SearchReadersQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Domain.Reader.Reader> readers =
            await readerReadRepository.Search(query.Name, query.Email, cancellationToken);

        if (readers.Count == 0)
        {
            return Result.Success<IReadOnlyCollection<ReaderSummaryResponse>>([]);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        // One query for every matched reader, then the counting happens in memory: whether a
        // reservation still holds its book depends on elapsed time, which only the domain knows.
        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByReaders(
                readers.Select(reader => reader.Id).ToArray(), cancellationToken);

        Dictionary<Guid, int> activeCounts = open
            .Where(reservation => reservation.IsActive(utcNow))
            .GroupBy(reservation => reservation.ReaderId)
            .ToDictionary(group => group.Key, group => group.Count());

        return Result.Success<IReadOnlyCollection<ReaderSummaryResponse>>(
            readers
                .Select(reader => ReaderSummaryResponse.FromDomain(
                    reader,
                    activeCounts.GetValueOrDefault(reader.Id)))
                .ToArray());
    }
}
