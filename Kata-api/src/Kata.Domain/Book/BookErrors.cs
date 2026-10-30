using SharedKernel;

namespace Kata.Domain.Book;

public class BookErrors
{
    public static Error NotFound => new("Book.NotFound", "Book not found.", ErrorType.NotFound);
    public static Error TitleRequired => new("Book.TitleRequired", "Book title not required", ErrorType.NotFound);
}