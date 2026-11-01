using Kata.Application.Features.Reader;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Kata.infrastructure.Features.Reader;

internal sealed class EfReaderRepository(KataDbContext dbContext) : IReaderReadRepository
{
    public async Task<bool> Exists(Guid id, CancellationToken cancellation = default) =>
        await dbContext.Readers.AnyAsync(reader => reader.Id == id, cancellation);
}
