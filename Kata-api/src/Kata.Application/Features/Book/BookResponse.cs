namespace Kata.Application.Features.Book;

public sealed record BookResponse(
    Guid Id,
    string Title,
    string? Author,
    int? PublicationYear)
{
    public static BookResponse FromDomain(Domain.Book.Book book) =>
        new(book.Id, book.Title, book.Author, book.PublicationYear);
}
