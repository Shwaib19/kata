using Kata.Application.Features.Book;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Kata.infrastructure.Features.Book;

internal sealed class EfBookRepository(KataDbContext dbContext) : IBookReadRepository, IBookWriteRepository
{
    public async Task Add(Domain.Book.Book book, CancellationToken cancellation = default) =>
        await dbContext.Books.AddAsync(book, cancellation);

    public async Task<Domain.Book.Book?> GetById(Guid id, CancellationToken cancellation = default) =>
        await dbContext.Books.FirstOrDefaultAsync(book => book.Id == id, cancellation);

    public async Task SaveChanges(CancellationToken cancellation = default) =>
        await dbContext.SaveChangesAsync(cancellation);

    public async Task<IReadOnlyCollection<Domain.Book.Book>> Search(
        string? title, string? author, CancellationToken cancellation = default)
    {
        IQueryable<Domain.Book.Book> query = dbContext.Books.AsNoTracking();

        // ILike is PostgreSQL's case-insensitive LIKE, so the search works without
        // lowercasing both sides in memory.
        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(book => EF.Functions.ILike(book.Title, $"%{title.Trim()}%"));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(book => book.Author != null &&
                                        EF.Functions.ILike(book.Author, $"%{author.Trim()}%"));
        }

        return await query.OrderBy(book => book.Title).ToListAsync(cancellation);
    }
}
