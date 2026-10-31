using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Create;

public sealed record CreateBookCommand(
    string? Title,
    string? Author,
    int? PublicationYear) : ICommand<Guid>;
