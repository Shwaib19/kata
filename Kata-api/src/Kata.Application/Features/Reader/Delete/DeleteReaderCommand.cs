using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Delete;

public sealed record DeleteReaderCommand(Guid ReaderId) : ICommand;
