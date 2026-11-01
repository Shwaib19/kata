using System.Net.Mail;
using SharedKernel;

namespace Kata.Domain.Reader;

public class Reader : ISoftDeletable
{
    public const int NameMaxLength = 100;

    public Guid Id { get; init; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    /// <summary>
    /// Replaces the reader's details. The same rules as <see cref="Create"/> apply, and a
    /// rejected update leaves the reader exactly as it was.
    /// </summary>
    public Result Update(string? name, string? email) =>
        throw new NotImplementedException();

    /// <summary>
    /// Takes the reader out of circulation without erasing it, so that past reservations
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
    private Reader()
    {
        Name = null!;
        Email = null!;
    }

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
