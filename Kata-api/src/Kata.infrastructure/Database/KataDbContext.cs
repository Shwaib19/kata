using Kata.Domain.Book;
using Kata.Domain.Reader;
using Kata.Domain.Reservation;
using Microsoft.EntityFrameworkCore;

namespace Kata.infrastructure.Database;

public sealed class KataDbContext(DbContextOptions<KataDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KataDbContext).Assembly);
    }
}
