using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Delete;

public sealed record DeleteBookCommand(Guid BookId) : ICommand;
