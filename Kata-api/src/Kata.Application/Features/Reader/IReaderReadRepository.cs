namespace Kata.Application.Features.Reader;

public interface IReaderReadRepository
{
    Task<bool> Exists(Guid id, CancellationToken cancellation = default);
}
