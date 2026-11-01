using Kata.Application.Features.Reader;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Kata.infrastructure.Features.Reader;

internal sealed class EfReaderRepository(KataDbContext dbContext)
    : IReaderReadRepository, IReaderWriteRepository
{
    public async Task Add(Domain.Reader.Reader reader, CancellationToken cancellation = default) =>
        await dbContext.Readers.AddAsync(reader, cancellation);

    public async Task SaveChanges(CancellationToken cancellation = default) =>
        await dbContext.SaveChangesAsync(cancellation);

    public async Task<bool> Exists(Guid id, CancellationToken cancellation = default) =>
        await dbContext.Readers.AnyAsync(reader => reader.Id == id, cancellation);

    public async Task<bool> ExistsByEmail(string email, CancellationToken cancellation = default) =>
        await dbContext.Readers.AnyAsync(reader => reader.Email == email, cancellation);

    public async Task<bool> EmailBelongsToAnotherReader(
        string email, Guid readerId, CancellationToken cancellation = default) =>
        await dbContext.Readers.AnyAsync(
            reader => reader.Email == email && reader.Id != readerId, cancellation);

    public async Task<Domain.Reader.Reader?> GetById(Guid id, CancellationToken cancellation = default) =>
        await dbContext.Readers.FirstOrDefaultAsync(reader => reader.Id == id, cancellation);

    public async Task<IReadOnlyCollection<Domain.Reader.Reader>> Search(
        string? name, string? email, CancellationToken cancellation = default)
    {
        IQueryable<Domain.Reader.Reader> query = dbContext.Readers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(reader => EF.Functions.ILike(reader.Name, $"%{name.Trim()}%"));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(reader => EF.Functions.ILike(reader.Email, $"%{email.Trim()}%"));
        }

        return await query.OrderBy(reader => reader.Name).ToListAsync(cancellation);
    }
}
