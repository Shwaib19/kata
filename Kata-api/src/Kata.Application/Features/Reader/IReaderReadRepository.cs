namespace Kata.Application.Features.Reader;

public interface IReaderReadRepository
{
    Task<bool> Exists(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// The email is expected already normalised, as <see cref="Domain.Reader.Reader.Create"/> stores it.
    /// </summary>
    Task<bool> ExistsByEmail(string email, CancellationToken cancellation = default);

    /// <summary>
    /// Whether the email is already held by a reader other than <paramref name="readerId"/>,
    /// so that a reader keeping its own email is not reported as a conflict.
    /// </summary>
    Task<bool> EmailBelongsToAnotherReader(
        string email, Guid readerId, CancellationToken cancellation = default);

    Task<Domain.Reader.Reader?> GetById(Guid id, CancellationToken cancellation = default);

    /// <summary>
    /// Returns the readers whose name or email contains the given fragments, case-insensitively.
    /// A null or blank criterion is ignored, so passing none returns everyone.
    /// </summary>
    Task<IReadOnlyCollection<Domain.Reader.Reader>> Search(
        string? name, string? email, CancellationToken cancellation = default);
}
