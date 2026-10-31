namespace Kata.Domain.Reader;

public class Reader
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public string Email { get; init; }
}