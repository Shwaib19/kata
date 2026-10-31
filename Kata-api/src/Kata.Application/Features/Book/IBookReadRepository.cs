namespace Kata.Application.Features.Book;

public interface IBookReadRepository
{
    Task<Domain.Book.Book?> GetById(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// Returns the books whose title or author contains the given fragments, case-insensitively.
    /// A null or blank criterion is ignored, so passing none returns the whole catalogue.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Book.Book>> Search(
        string? title, string? author, CancellationToken cancellation = default);
}
