namespace Kata.Application.Features.Reader;

public interface IReaderWriteRepository
{
    Task Add(Domain.Reader.Reader reader, CancellationToken cancellation = default);

    /// <summary>
    /// Returns a tracked reader, so that changes made on it are picked up by <see cref="SaveChanges"/>.
    /// </summary>
    Task<Domain.Reader.Reader?> GetById(Guid id, CancellationToken cancellation = default);

    Task SaveChanges(CancellationToken cancellation = default);
}
