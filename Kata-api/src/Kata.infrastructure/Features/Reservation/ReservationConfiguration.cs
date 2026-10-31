using Kata.Domain.Reservation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kata.infrastructure.Features.Reservation;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Domain.Reservation.Reservation>
{
    public void Configure(EntityTypeBuilder<Domain.Reservation.Reservation> builder)
    {
        builder.HasKey(reservation => reservation.Id);

        builder.Property(reservation => reservation.Status)
            .IsRequired();

        builder.HasIndex(reservation => reservation.ReaderId);

        // Foreign keys without navigation properties: the database enforces integrity
        // while the domain keeps referring to books and readers by identifier only.
        // Deletion is restricted rather than cascading: erasing a book must not silently
        // erase the record of who borrowed it. The use case turns the refusal into a 409.
        builder.HasOne<Kata.Domain.Book.Book>()
            .WithMany()
            .HasForeignKey(reservation => reservation.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Kata.Domain.Reader.Reader>()
            .WithMany()
            .HasForeignKey(reservation => reservation.ReaderId)
            .OnDelete(DeleteBehavior.Restrict);

        // A book can appear at most once among the reservations that still hold it.
        // The filter is what makes this a history table: cancelled, returned and expired
        // rows are ignored, so the same book can be reserved again and again over time.
        // This index — not the application code — is what rejects two simultaneous
        // reservations on the same book.
        builder.HasIndex(reservation => reservation.BookId)
            .IsUnique()
            .HasFilter($"\"Status\" IN ({(int)ReservationStatus.Reserved}, {(int)ReservationStatus.Borrowed})");
    }
}
