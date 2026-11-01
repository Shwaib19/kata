using Kata.Application.Features.Reservation;
using Kata.Domain.Book;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.GetById;

internal sealed class GetBookByIdQueryHandler(
    IBookReadRepository bookReadRepository,
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetBookByIdQuery, BookResponse>
{
    public async Task<Result<BookResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.Book.Book? book = await bookReadRepository.GetById(query.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<BookResponse>(BookErrors.NotFound);
        }

        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByBooks([book.Id], cancellationToken);

        bool isAvailable = !open.Any(reservation => reservation.IsActive(dateTimeProvider.UtcNow));

        return Result.Success(BookResponse.FromDomain(book, isAvailable));
    }
}
