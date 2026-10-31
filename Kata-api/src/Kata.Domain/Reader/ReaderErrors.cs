using SharedKernel;

namespace Kata.Domain.Reader;

public static class ReaderErrors
{
    public static Error NotFound => new(
        "Reader.NotFound",
        "Reader not found.",
        ErrorType.NotFound);

    public static Error NameRequired => new(
        "Reader.NameRequired",
        "A reader name is required.",
        ErrorType.Validation);

    public static Error NameTooLong => new(
        "Reader.NameTooLong",
        $"A reader name must not exceed {Reader.NameMaxLength} characters.",
        ErrorType.Validation);

    public static Error EmailRequired => new(
        "Reader.EmailRequired",
        "A reader email is required.",
        ErrorType.Validation);

    public static Error EmailInvalid => new(
        "Reader.EmailInvalid",
        "A reader email must be a valid email address.",
        ErrorType.Validation);
}
