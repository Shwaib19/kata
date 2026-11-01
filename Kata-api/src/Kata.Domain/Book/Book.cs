using SharedKernel;

namespace Kata.Domain.Book;

public class Book : ISoftDeletable
{
    public const int TitleMaxLength = 50;

    public Guid Id { get; init; }
    public string Title { get; private set; }
    public string? Author { get; private set; }
    public int? PublicationYear { get; init; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    /// <summary>
    /// Replaces the book's details. The same rules as <see cref="Create"/> apply, and a
    /// rejected update leaves the book exactly as it was.
    /// </summary>
    public Result Update(
        string? title, string? author, int? publicationYear, IDateTimeProvider dateTimeProvider) =>
        throw new NotImplementedException();

    /// <summary>
    /// Takes the book out of circulation without erasing it, so that past reservations
    /// keep pointing at something. Calling it twice keeps the first deletion date.
    /// </summary>
    public void Delete()
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedOnUtc = DateTime.UtcNow;
    }

    // Required by EF Core, which materialises entities without going through Create.
    private Book()
    {
        Title = null!;
    }

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
