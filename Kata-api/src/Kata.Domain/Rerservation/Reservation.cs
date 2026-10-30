namespace Kata.Domain;

public class Reservation
{
     Guid Id { get; init; }
     Guid BookId { get; init; }
     Guid ReaderId { get; init; }
}