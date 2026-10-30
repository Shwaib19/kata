using Kata.Domain;
using Kata.Domain.Book;
using SharedKernel;

namespace Kata.test;

public class BookTest
{
    private const string BookTitle = "Book Title";
    private const string BookAuthor = "Book Author";
    private const int PublicationYear = 2005;

    [Fact]
    public void Create_Should_Create_Book_With_The_Title_Author_And_Publication_Year()
    {
        Result<Book> result = Book.Create(BookTitle, BookAuthor, PublicationYear);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(BookTitle, result.Value.Title);
        Assert.Equal(BookAuthor, result.Value.Author);
        Assert.Equal(PublicationYear, result.Value.PublicationYear);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Leave_Author_Unknown_When_Author_Is_Missing(string? missingAuthor)
    {
        Result<Book> result = Book.Create(BookTitle, missingAuthor, PublicationYear);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Author);
    }

    [Fact]
    public void Create_Should_Leave_Publication_Year_Unknown_When_Publication_Year_Is_Missing()
    {
        Result<Book> result = Book.Create(BookTitle, BookAuthor, null);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.PublicationYear);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Title_Is_Missing(string? missingTitle)
    {
        Result<Book> result = Book.Create(missingTitle, BookAuthor, PublicationYear);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.TitleRequired, result.Error);
    }
}
