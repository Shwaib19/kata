using Kata.Application;
using Kata.Domain.Book;
using Kata.Domain.Reader;
using Kata.infrastructure;
using Kata.infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace Kata.IntegrationTests;

/// <summary>
/// A real PostgreSQL database and the application wired on top of it.
///
/// These tests exist because the rule they check lives in the database, not in C#: a unique
/// index is what forbids two readers from holding the same book, and no in-memory provider
/// enforces it. The application is composed exactly as the API composes it, minus HTTP.
/// </summary>
public sealed class LibraryDatabase : IAsyncLifetime
{
    /// <summary>
    /// A database of its own, never the one the application runs on: these tests empty the
    /// tables between cases. Overridable so the pipeline can point at its own service.
    /// </summary>
    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("KATA_TEST_DATABASE")
        ?? "Host=localhost;Port=5432;Database=kata_tests;Username=kata;Password=kata";

    public ServiceProvider Services { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = ConnectionString,
            })
            .Build();

        ServiceCollection services = new();
        services.AddApplication();
        services.AddInfrastructure(configuration);

        Services = services.BuildServiceProvider();

        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        KataDbContext dbContext = scope.ServiceProvider.GetRequiredService<KataDbContext>();

        // Creates the database on first run, and brings the schema up to date afterwards,
        // which is what guarantees the unique index under test is actually there.
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await Services.DisposeAsync();

    /// <summary>
    /// Leaves the tables empty, so one case cannot be read as the result of another.
    /// </summary>
    public async Task Reset()
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        KataDbContext dbContext = scope.ServiceProvider.GetRequiredService<KataDbContext>();

        await dbContext.Database.ExecuteSqlRawAsync(
            """TRUNCATE "Reservations", "Books", "Readers" """);
    }

    /// <summary>
    /// One book and two readers: the smallest situation in which two reservations can compete.
    /// </summary>
    public async Task<(Guid BookId, Guid FirstReaderId, Guid SecondReaderId)> Seed()
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        KataDbContext dbContext = scope.ServiceProvider.GetRequiredService<KataDbContext>();
        IDateTimeProvider clock = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        Book book = Book.Create("Dune", "Frank Herbert", 1965, clock).Value;
        Reader first = Reader.Create("Ada Lovelace", "ada@example.com").Value;
        Reader second = Reader.Create("Grace Hopper", "grace@example.com").Value;

        dbContext.Books.Add(book);
        dbContext.Readers.AddRange(first, second);

        await dbContext.SaveChangesAsync();

        return (book.Id, first.Id, second.Id);
    }

    public async Task<int> CountActiveReservations(Guid bookId)
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        KataDbContext dbContext = scope.ServiceProvider.GetRequiredService<KataDbContext>();

        return await dbContext.Reservations
            .CountAsync(reservation => reservation.BookId == bookId &&
                                       (reservation.Status == Domain.Reservation.ReservationStatus.Reserved ||
                                        reservation.Status == Domain.Reservation.ReservationStatus.Borrowed));
    }
}
