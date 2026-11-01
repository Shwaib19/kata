using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Search;

/// <summary>
/// Both criteria are optional: passing neither lists the whole catalogue.
/// </summary>
public sealed record SearchBooksQuery(
    string? Title,
    string? Author) : IQuery<IReadOnlyCollection<BookResponse>>;
