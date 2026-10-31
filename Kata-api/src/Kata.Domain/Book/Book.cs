using SharedKernel;

namespace Kata.Domain.Book;

public class Book
{
    public const int TitleMaxLength = 50;

    public Guid Id { get; init; }
    public string Title { get; private set; }
    public string? Author { get; private set; }
    public int? PublicationYear { get; init; }

    private Book(string title, string? author, int? publicationYear)
    {
        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        PublicationYear = publicationYear;
    }

    public static Result<Book> Create(
        string? title,
        string? author,
        int? publicationYear,
        IDateTimeProvider dateTimeProvider)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Book>(BookErrors.TitleRequired);
        }

        var trimmedTitle = title.Trim();

        if (trimmedTitle.Length > TitleMaxLength)
        {
            return Result.Failure<Book>(BookErrors.TitleTooLong);
        }

        if (publicationYear > dateTimeProvider.UtcNow.Year)
        {
            return Result.Failure<Book>(BookErrors.PublicationYearInFuture);
        }

        var trimmedAuthor = string.IsNullOrWhiteSpace(author) ? null : author.Trim();

        return Result.Success(new Book(trimmedTitle, trimmedAuthor, publicationYear));
    }
}
