using Kata.Domain.Book;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.GetById;

internal sealed class GetBookByIdQueryHandler(IBookReadRepository bookReadRepository)
    : IQueryHandler<GetBookByIdQuery, BookResponse>
{
    public async Task<Result<BookResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.Book.Book? book = await bookReadRepository.GetById(query.BookId, cancellationToken);

        return book is null
            ? Result.Failure<BookResponse>(BookErrors.NotFound)
            : Result.Success(BookResponse.FromDomain(book));
    }
}
