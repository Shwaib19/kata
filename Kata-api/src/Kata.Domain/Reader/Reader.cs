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
    public Result Update(string? name, string? email)
    {
        Result<Details> details = Validate(name, email);

        if (details.IsFailure)
        {
            return Result.Failure(details.Error);
        }

        Name = details.Value.Name;
        Email = details.Value.Email;

        return Result.Success();
    }

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
        Result<Details> details = Validate(name, email);

        return details.IsFailure
            ? Result.Failure<Reader>(details.Error)
            : Result.Success(new Reader(details.Value.Name, details.Value.Email));
    }

    /// <summary>
    /// The details a reader is allowed to hold, once checked and normalised. Creating and
    /// updating share this so that both enforce the same rules, and so that an update writes
    /// nothing until every rule has passed.
    /// </summary>
    private readonly record struct Details(string Name, string Email);

    /// <summary>
    /// How an email is stored, so that a caller needing to look one up compares it the same
    /// way this entity does rather than reimplementing the rule.
    /// </summary>
    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static Result<Details> Validate(string? name, string? email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Details>(ReaderErrors.NameRequired);
        }

        string trimmedName = name.Trim();

        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Failure<Details>(ReaderErrors.NameTooLong);
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Details>(ReaderErrors.EmailRequired);
        }

        string normalizedEmail = NormalizeEmail(email);

        if (!MailAddress.TryCreate(normalizedEmail, out MailAddress? address) ||
            address.Address != normalizedEmail)
        {
            return Result.Failure<Details>(ReaderErrors.EmailInvalid);
        }

        return Result.Success(new Details(trimmedName, normalizedEmail));
    }
}
