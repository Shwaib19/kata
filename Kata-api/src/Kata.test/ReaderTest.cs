using Kata.Domain.Reader;
using SharedKernel;

namespace Kata.test;

public class ReaderTest
{
    private const string ReaderName = "Ada KONE";
    private const string ReaderEmail = "ada@example.com";

    [Fact]
    public void Create_Should_Create_Reader_With_The_Name_And_Email()
    {
        Result<Reader> result = Reader.Create(ReaderName, ReaderEmail);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(ReaderName, result.Value.Name);
        Assert.Equal(ReaderEmail, result.Value.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Name_Is_Missing(string? missingName)
    {
        Result<Reader> result = Reader.Create(missingName, ReaderEmail);

        Assert.True(result.IsFailure);
        Assert.Equal(ReaderErrors.NameRequired, result.Error);
    }

    [Fact]
    public void Create_Should_Fail_When_Name_Is_Longer_Than_The_Max_Length()
    {
        string tooLongName = new('a', Reader.NameMaxLength + 1);

        Result<Reader> result = Reader.Create(tooLongName, ReaderEmail);

        Assert.True(result.IsFailure);
        Assert.Equal(ReaderErrors.NameTooLong, result.Error);
    }

    [Fact]
    public void Create_Should_Succeed_When_Name_Is_Exactly_The_Max_Length()
    {
        string longestAllowedName = new('a', Reader.NameMaxLength);

        Result<Reader> result = Reader.Create(longestAllowedName, ReaderEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(longestAllowedName, result.Value.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Should_Fail_When_Email_Is_Missing(string? missingEmail)
    {
        Result<Reader> result = Reader.Create(ReaderName, missingEmail);

        Assert.True(result.IsFailure);
        Assert.Equal(ReaderErrors.EmailRequired, result.Error);
    }

    [Theory]
    [InlineData("ada")]
    [InlineData("ada@")]
    [InlineData("@example.com")]
    [InlineData("ada example.com")]
    [InlineData("ada@@example.com")]
    public void Create_Should_Fail_When_Email_Is_Not_A_Valid_Address(string invalidEmail)
    {
        Result<Reader> result = Reader.Create(ReaderName, invalidEmail);

        Assert.True(result.IsFailure);
        Assert.Equal(ReaderErrors.EmailInvalid, result.Error);
    }

    [Fact]
    public void Create_Should_Trim_The_Name_And_The_Email()
    {
        Result<Reader> result = Reader.Create($"  {ReaderName}  ", $"  {ReaderEmail}  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(ReaderName, result.Value.Name);
        Assert.Equal(ReaderEmail, result.Value.Email);
    }

    [Theory]
    [InlineData("ADA@EXAMPLE.COM")]
    [InlineData("Ada@Example.Com")]
    public void Create_Should_Lowercase_The_Email(string mixedCaseEmail)
    {
        Result<Reader> result = Reader.Create(ReaderName, mixedCaseEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(ReaderEmail, result.Value.Email);
    }

    [Fact]
    public void Delete_Should_Take_The_Reader_Off_The_Register()
    {
        Reader reader = Reader.Create(ReaderName, ReaderEmail).Value;

        reader.Delete();

        Assert.True(reader.IsDeleted);
        Assert.NotNull(reader.DeletedOnUtc);
    }
}
