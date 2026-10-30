namespace Kata.Domain.Book;

public class Book
{
    public Guid  Id { get; init; }
    public string Title { get; private set; }
    public string? Author { get; private set; }
    public int? PublicationYear  { get; init; }
}