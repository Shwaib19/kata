using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.GetById;

public sealed record GetBookByIdQuery(Guid BookId) : IQuery<BookResponse>;
