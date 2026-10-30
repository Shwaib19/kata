using SharedKernel;

namespace Kata.Domain.Reader;

public class ReaderErrors
{
    public static Error NotFound => new("Book.NotFound", "Book not found.", ErrorType.NotFound);
}