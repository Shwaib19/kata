using Kata.Application.Features.Book;
using SharedKernel;

namespace Kata.infrastructure.Features.Book;

public sealed class EfBookRepository : IBookReadRepository, IBookWriteRepository
{
    public Task<Domain.Book.Book?> GetById(string id, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }


    public Task<Result<Domain.Book.Book>> Add(Domain.Book.Book book, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}