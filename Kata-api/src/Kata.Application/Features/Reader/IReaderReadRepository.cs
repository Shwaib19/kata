namespace Kata.Application.Features.Reader;

public interface IReaderReadRepository
{
    Task<bool> Exists(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// The email is expected already normalised, as <see cref="Domain.Reader.Reader.Create"/> stores it.
    /// </summary>
    Task<bool> ExistsByEmail(string email, CancellationToken cancellation = default);

    Task<Domain.Reader.Reader?> GetById(Guid id, CancellationToken cancellation = default);
}
