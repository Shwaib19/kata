using SharedKernel;

namespace Kata.Domain.Book;

public class Book : ISoftDeletable
{
    public const int TitleMaxLength = 50;

    public Guid Id { get; init; }
    public string Title { get; private set; }
    public string? Author { get; private set; }
    public int? PublicationYear { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    /// <summary>
    /// Replaces the book's details. The same rules as <see cref="Create"/> apply, and a
    /// rejected update leaves the book exactly as it was.
    /// </summary>
    public Result Update(
        string? title, string? author, int? publicationYear, IDateTimeProvider dateTimeProvider)
    {
        Result<Details> details = Validate(title, author, publicationYear, dateTimeProvider);

        if (details.IsFailure)
        {
            return Result.Failure(details.Error);
        }

        Title = details.Value.Title;
        Author = details.Value.Author;
        PublicationYear = details.Value.PublicationYear;

        return Result.Success();
    }

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
        Result<Details> details = Validate(title, author, publicationYear, dateTimeProvider);

        return details.IsFailure
            ? Result.Failure<Book>(details.Error)
            : Result.Success(new Book(details.Value.Title, details.Value.Author, details.Value.PublicationYear));
    }

    /// <summary>
    /// The details a book is allowed to hold, once checked and normalised. Creating and updating
    /// share this so that both enforce the same rules, and so that an update writes nothing
    /// until every rule has passed.
    /// </summary>
    private readonly record struct Details(string Title, string? Author, int? PublicationYear);

    private static Result<Details> Validate(
        string? title,
        string? author,
        int? publicationYear,
        IDateTimeProvider dateTimeProvider)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Details>(BookErrors.TitleRequired);
        }

        var trimmedTitle = title.Trim();

        if (trimmedTitle.Length > TitleMaxLength)
        {
            return Result.Failure<Details>(BookErrors.TitleTooLong);
        }

        if (publicationYear > dateTimeProvider.UtcNow.Year)
        {
            return Result.Failure<Details>(BookErrors.PublicationYearInFuture);
        }

        var trimmedAuthor = string.IsNullOrWhiteSpace(author) ? null : author.Trim();

        return Result.Success(new Details(trimmedTitle, trimmedAuthor, publicationYear));
    }
}
