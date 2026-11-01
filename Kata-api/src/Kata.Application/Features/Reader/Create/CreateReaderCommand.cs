using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Create;

public sealed record CreateReaderCommand(
    string? Name,
    string? Email) : ICommand<Guid>;
