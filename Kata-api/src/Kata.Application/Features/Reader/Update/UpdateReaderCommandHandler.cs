using Kata.Domain.Reader;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Update;

internal sealed class UpdateReaderCommandHandler(
    IReaderReadRepository readerReadRepository,
    IReaderWriteRepository readerWriteRepository)
    : ICommandHandler<UpdateReaderCommand>
{
    public async Task<Result> Handle(UpdateReaderCommand command, CancellationToken cancellationToken)
    {
        Domain.Reader.Reader? reader = await readerWriteRepository.GetById(command.ReaderId, cancellationToken);

        if (reader is null)
        {
            return Result.Failure(ReaderErrors.NotFound);
        }

        // Checked before the entity is touched, and compared the way the domain stores an email.
        // A reader keeping its own email is not a conflict, hence the exclusion.
        if (!string.IsNullOrWhiteSpace(command.Email) &&
            await readerReadRepository.EmailBelongsToAnotherReader(
                Domain.Reader.Reader.NormalizeEmail(command.Email), reader.Id, cancellationToken))
        {
            return Result.Failure(ReaderErrors.EmailAlreadyUsed);
        }

        Result updated = reader.Update(command.Name, command.Email);

        if (updated.IsFailure)
        {
            return updated;
        }

        await readerWriteRepository.SaveChanges(cancellationToken);

        return Result.Success();
    }
}
