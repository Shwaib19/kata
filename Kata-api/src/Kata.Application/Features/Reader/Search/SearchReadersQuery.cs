using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Search;

/// <summary>
/// Both criteria are optional: passing neither lists every reader.
/// </summary>
public sealed record SearchReadersQuery(
    string? Name,
    string? Email) : IQuery<IReadOnlyCollection<ReaderSummaryResponse>>;
