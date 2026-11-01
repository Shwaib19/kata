using Kata.Domain.Reader;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Create;

internal sealed class CreateReaderCommandHandler(
    IReaderReadRepository readerReadRepository,
    IReaderWriteRepository readerWriteRepository)
    : ICommandHandler<CreateReaderCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateReaderCommand command, CancellationToken cancellationToken)
    {
        Result<Domain.Reader.Reader> reader = Domain.Reader.Reader.Create(command.Name, command.Email);

        if (reader.IsFailure)
        {
            return Result.Failure<Guid>(reader.Error);
        }

        // Checked against the email the domain stored, not the one that was typed: Create
        // trims and lowercases it, so "  Alice@Example.COM  " and "alice@example.com" collide.
        if (await readerReadRepository.ExistsByEmail(reader.Value.Email, cancellationToken))
        {
            return Result.Failure<Guid>(ReaderErrors.EmailAlreadyUsed);
        }

        await readerWriteRepository.Add(reader.Value, cancellationToken);
        await readerWriteRepository.SaveChanges(cancellationToken);

        return Result.Success(reader.Value.Id);
    }
}
