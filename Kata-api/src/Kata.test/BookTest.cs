using Kata.Domain.Book;
using SharedKernel;

namespace Kata.test;

public class BookTest
{
    private const string BookTitle = "Book Title";
    private const string BookAuthor = "Book Author";
    private const int PublicationYear = 2005;

    private static readonly DateTime Now = new(2026, 10, 31, 12, 0, 0, DateTimeKind.Utc);

    private readonly IDateTimeProvider _dateTimeProvider = new FakeDateTimeProvider(Now);

    [Fact]
    public void Create_Should_Create_Book_With_The_Title_Author_And_Publication_Year()
    {
        Result<Book> result = Book.Create(BookTitle, BookAuthor, PublicationYear, _dateTimeProvider);

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
        Result<Book> result = Book.Create(BookTitle, missingAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Author);
    }

    [Fact]
    public void Create_Should_Leave_Publication_Year_Unknown_When_Publication_Year_Is_Missing()
    {
        Result<Book> result = Book.Create(BookTitle, BookAuthor, null, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.PublicationYear);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Title_Is_Missing(string? missingTitle)
    {
        Result<Book> result = Book.Create(missingTitle, BookAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.TitleRequired, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_When_Title_Is_Longer_Than_The_Max_Length()
    {
        string tooLongTitle = new('a', Book.TitleMaxLength + 1);

        Result<Book> result = Book.Create(tooLongTitle, BookAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.TitleTooLong, result.Error);
    }

    [Fact]
    public void Create_Should_Succeed_When_Title_Is_Exactly_The_Max_Length()
    {
        string longestAllowedTitle = new('a', Book.TitleMaxLength);

        Result<Book> result = Book.Create(longestAllowedTitle, BookAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal(longestAllowedTitle, result.Value.Title);
    }

    [Fact]
    public void Create_Should_Trim_The_Title_And_The_Author()
    {
        Result<Book> result = Book.Create($"  {BookTitle}  ", $"  {BookAuthor}  ", PublicationYear, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal(BookTitle, result.Value.Title);
        Assert.Equal(BookAuthor, result.Value.Author);
    }

    [Fact]
    public void Create_Should_Measure_The_Title_Max_Length_After_Trimming()
    {
        string longestAllowedTitle = new('a', Book.TitleMaxLength);

        Result<Book> result = Book.Create($"  {longestAllowedTitle}  ", BookAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal(longestAllowedTitle, result.Value.Title);
    }

    [Fact]
    public void Create_Should_Fail_When_Publication_Year_Is_In_The_Future()
    {
        Result<Book> result = Book.Create(BookTitle, BookAuthor, Now.Year + 1, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.PublicationYearInFuture, result.Error);
    }

    [Fact]
    public void Create_Should_Succeed_When_Publication_Year_Is_The_Current_Year()
    {
        Result<Book> result = Book.Create(BookTitle, BookAuthor, Now.Year, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal(Now.Year, result.Value.PublicationYear);
    }

    [Fact]
    public void Delete_Should_Take_The_Book_Out_Of_The_Catalogue()
    {
        Book book = Book.Create(BookTitle, BookAuthor, PublicationYear, _dateTimeProvider).Value;

        book.Delete();

        Assert.True(book.IsDeleted);
        Assert.NotNull(book.DeletedOnUtc);
    }

    [Fact]
    public void Delete_Should_Keep_The_First_Deletion_Date_When_Called_Twice()
    {
        Book book = Book.Create(BookTitle, BookAuthor, PublicationYear, _dateTimeProvider).Value;
        book.Delete();
        DateTime? firstDeletion = book.DeletedOnUtc;

        book.Delete();

        Assert.Equal(firstDeletion, book.DeletedOnUtc);
    }

    [Fact]
    public void Update_Should_Replace_The_Title_Author_And_Publication_Year()
    {
        Book book = ABook();

        Result result = book.Update("New Title", "New Author", 1999, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", book.Title);
        Assert.Equal("New Author", book.Author);
        Assert.Equal(1999, book.PublicationYear);
    }

    [Fact]
    public void Update_Should_Keep_The_Identifier()
    {
        Book book = ABook();
        Guid idBeforeUpdate = book.Id;

        book.Update("New Title", "New Author", 1999, _dateTimeProvider);

        Assert.Equal(idBeforeUpdate, book.Id);
    }

    [Fact]
    public void Update_Should_Trim_The_Title_And_The_Author()
    {
        Book book = ABook();

        Result result = book.Update("  New Title  ", "  New Author  ", 1999, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", book.Title);
        Assert.Equal("New Author", book.Author);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_Leave_Author_Unknown_When_Author_Is_Missing(string? missingAuthor)
    {
        Book book = ABook();

        Result result = book.Update(BookTitle, missingAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsSuccess);
        Assert.Null(book.Author);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_Fail_When_Title_Is_Missing(string? missingTitle)
    {
        Book book = ABook();

        Result result = book.Update(missingTitle, BookAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.TitleRequired, result.Error);
    }

    [Fact]
    public void Update_Should_Fail_When_Title_Is_Longer_Than_The_Max_Length()
    {
        Book book = ABook();
        string tooLongTitle = new('a', Book.TitleMaxLength + 1);

        Result result = book.Update(tooLongTitle, BookAuthor, PublicationYear, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.TitleTooLong, result.Error);
    }

    [Fact]
    public void Update_Should_Fail_When_Publication_Year_Is_In_The_Future()
    {
        Book book = ABook();

        Result result = book.Update(BookTitle, BookAuthor, Now.Year + 1, _dateTimeProvider);

        Assert.True(result.IsFailure);
        Assert.Equal(BookErrors.PublicationYearInFuture, result.Error);
    }

    [Fact]
    public void Update_Should_Leave_The_Book_Untouched_When_It_Fails()
    {
        Book book = ABook();

        book.Update(null, "New Author", 1999, _dateTimeProvider);

        Assert.Equal(BookTitle, book.Title);
        Assert.Equal(BookAuthor, book.Author);
        Assert.Equal(PublicationYear, book.PublicationYear);
    }

    private Book ABook() =>
        Book.Create(BookTitle, BookAuthor, PublicationYear, _dateTimeProvider).Value;
}
