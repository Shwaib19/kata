using SharedKernel;

namespace Kata.Application.Features.Book;

public interface IBookWriteRepository
{
    Task<Result<Domain.Book.Book>> Add(Domain.Book.Book book, CancellationToken cancellation = default);
}