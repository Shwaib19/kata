using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Update;

public sealed record UpdateReaderCommand(
    Guid ReaderId,
    string? Name,
    string? Email) : ICommand;
