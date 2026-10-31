namespace Kata.Application.Features.Book;

public interface IBookReadRepository
{
    Task<Domain.Book.Book?> GetById(string id, CancellationToken cancellation = default);
    
    
}