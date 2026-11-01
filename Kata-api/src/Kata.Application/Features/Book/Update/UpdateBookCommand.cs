using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Update;

public sealed record UpdateBookCommand(
    Guid BookId,
    string? Title,
    string? Author,
    int? PublicationYear) : ICommand;
