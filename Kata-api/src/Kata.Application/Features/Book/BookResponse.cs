namespace Kata.Application.Features.Book;

public sealed record BookResponse(
    Guid Id,
    string Title,
    string? Author,
    int? PublicationYear,
    bool IsAvailable)
{
    /// <summary>
    /// Availability is passed in rather than read from the book: it is derived from the
    /// reservations holding it at this instant, and deliberately not stored on the book itself.
    /// </summary>
    public static BookResponse FromDomain(Domain.Book.Book book, bool isAvailable) =>
        new(book.Id, book.Title, book.Author, book.PublicationYear, isAvailable);
}
