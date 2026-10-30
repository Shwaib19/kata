namespace Kata.Domain.Reader;

public class Reader
{
    Guid Id { get; init; }
    public string Name { get; private set; }
    public string Email { get; init; }
}