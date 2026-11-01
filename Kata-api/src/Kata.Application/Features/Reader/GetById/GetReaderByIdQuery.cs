using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.GetById;

public sealed record GetReaderByIdQuery(Guid ReaderId) : IQuery<ReaderResponse>;
