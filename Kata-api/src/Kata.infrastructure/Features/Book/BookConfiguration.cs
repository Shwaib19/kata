using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kata.infrastructure.Features.Book;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Domain.Book.Book>
{
    public void Configure(EntityTypeBuilder<Domain.Book.Book> builder)
    {
        builder.HasKey(book => book.Id);

        // Deleting a book takes it out of the catalogue without erasing it, so that past
        // reservations keep their meaning. This filter hides deleted rows from every query;
        // a query that must see them, such as a reader's history, asks for IgnoreQueryFilters.
        builder.HasQueryFilter(book => !book.IsDeleted);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(Domain.Book.Book.TitleMaxLength);

        builder.Property(book => book.Author)
            .HasMaxLength(Domain.Book.Book.TitleMaxLength);

        // Books are searched by title and by author.
        builder.HasIndex(book => book.Title);
        builder.HasIndex(book => book.Author);
    }
}
