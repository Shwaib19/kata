using Kata.Application.Features.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Search;

internal sealed class SearchBooksQueryHandler(
    IBookReadRepository bookReadRepository,
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<SearchBooksQuery, IReadOnlyCollection<BookResponse>>
{
    public async Task<Result<IReadOnlyCollection<BookResponse>>> Handle(
        SearchBooksQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Domain.Book.Book> books =
            await bookReadRepository.Search(query.Title, query.Author, cancellationToken);

        if (books.Count == 0)
        {
            return Result.Success<IReadOnlyCollection<BookResponse>>([]);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        // The reservations of every matched book are fetched in one query, then availability is
        // decided in memory. Asking per book would be as many round trips as there are results.
        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByBooks(
                books.Select(book => book.Id).ToArray(), cancellationToken);

        HashSet<Guid> heldBookIds = open
            .Where(reservation => reservation.IsActive(utcNow))
            .Select(reservation => reservation.BookId)
            .ToHashSet();

        return Result.Success<IReadOnlyCollection<BookResponse>>(
            books
                .Select(book => BookResponse.FromDomain(book, !heldBookIds.Contains(book.Id)))
                .ToArray());
    }
}
