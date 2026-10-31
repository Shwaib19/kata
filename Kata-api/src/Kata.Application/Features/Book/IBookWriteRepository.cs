namespace Kata.Application.Features.Book;

public interface IBookWriteRepository
{
    Task Add(Domain.Book.Book book, CancellationToken cancellation = default);

    /// <summary>
    /// Returns a tracked book, so that changes made on it are picked up by <see cref="SaveChanges"/>.
    /// </summary>
    Task<Domain.Book.Book?> GetById(Guid id, CancellationToken cancellation = default);

    Task SaveChanges(CancellationToken cancellation = default);
}
