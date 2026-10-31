using System.Net.Mail;
using SharedKernel;

namespace Kata.Domain.Reader;

public class Reader
{
    public const int NameMaxLength = 100;

    public Guid Id { get; init; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    private Reader(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }

    public static Result<Reader> Create(string? name, string? email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Reader>(ReaderErrors.NameRequired);
        }

        string trimmedName = name.Trim();

        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Failure<Reader>(ReaderErrors.NameTooLong);
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Reader>(ReaderErrors.EmailRequired);
        }

        string normalizedEmail = email.Trim().ToLowerInvariant();

        if (!MailAddress.TryCreate(normalizedEmail, out MailAddress? address) ||
            address.Address != normalizedEmail)
        {
            return Result.Failure<Reader>(ReaderErrors.EmailInvalid);
        }

        return Result.Success(new Reader(trimmedName, normalizedEmail));
    }
}
