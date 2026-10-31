using SharedKernel;

namespace Kata.Domain.Book;

public static class BookErrors
{
    public static Error NotFound => new(
        "Book.NotFound",
        "Book not found.",
        ErrorType.NotFound);

    public static Error TitleRequired => new(
        "Book.TitleRequired",
        "A book title is required.",
        ErrorType.Validation);

    public static Error TitleTooLong => new(
        "Book.TitleTooLong",
        $"A book title must not exceed {Book.TitleMaxLength} characters.",
        ErrorType.Validation);

    public static Error PublicationYearInFuture => new(
        "Book.PublicationYearInFuture",
        "A book publication year cannot be in the future.",
        ErrorType.Validation);
}
