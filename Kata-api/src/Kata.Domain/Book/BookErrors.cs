using SharedKernel;

namespace Kata.Domain.Book;

public static class BookErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Book.NotFound",
        "Book not found.");

    public static readonly Error TitleRequired = Error.BadRequest(
        "Book.TitleRequired",
        "A book title is required.");

    public static readonly Error TitleTooLong = Error.BadRequest(
        "Book.TitleTooLong",
        $"A book title must not exceed {Book.TitleMaxLength} characters.");

    public static readonly Error PublicationYearInFuture = Error.BadRequest(
        "Book.PublicationYearInFuture",
        "A book publication year cannot be in the future.");
}
