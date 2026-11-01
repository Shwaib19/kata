using Kata.Application.Features.Reservation;
using Kata.Domain.Reader;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reader.Delete;

internal sealed class DeleteReaderCommandHandler(
    IReaderWriteRepository readerWriteRepository,
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<DeleteReaderCommand>
{
    public async Task<Result> Handle(DeleteReaderCommand command, CancellationToken cancellationToken)
    {
        Domain.Reader.Reader? reader = await readerWriteRepository.GetById(command.ReaderId, cancellationToken);

        if (reader is null)
        {
            return Result.Failure(ReaderErrors.NotFound);
        }

        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByReader(reader.Id, cancellationToken);

        // A reader still holding a book has to give it back first, otherwise the loan would
        // belong to nobody.
        if (open.Any(reservation => reservation.IsActive(dateTimeProvider.UtcNow)))
        {
            return Result.Failure(ReaderErrors.HasActiveReservation);
        }

        reader.Delete();

        await readerWriteRepository.SaveChanges(cancellationToken);

        return Result.Success();
    }
}
