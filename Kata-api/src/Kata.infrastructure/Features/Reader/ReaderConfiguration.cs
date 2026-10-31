using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kata.infrastructure.Features.Reader;

internal sealed class ReaderConfiguration : IEntityTypeConfiguration<Domain.Reader.Reader>
{
    public void Configure(EntityTypeBuilder<Domain.Reader.Reader> builder)
    {
        builder.HasKey(reader => reader.Id);

        // Deleting a reader takes it out of the catalogue without erasing it, so that past
        // reservations keep their meaning. This filter hides deleted rows from every query;
        // a query that must see them, such as a reader's history, asks for IgnoreQueryFilters.
        builder.HasQueryFilter(reader => !reader.IsDeleted);

        builder.Property(reader => reader.Name)
            .IsRequired()
            .HasMaxLength(Domain.Reader.Reader.NameMaxLength);

        builder.Property(reader => reader.Email)
            .IsRequired()
            .HasMaxLength(320); // RFC 5321 maximum.

        // The brief requires reader emails to be unique. Reader.Create lowercases them,
        // so a plain unique index is enough to enforce it.
        builder.HasIndex(reader => reader.Email)
            .IsUnique();
    }
}
