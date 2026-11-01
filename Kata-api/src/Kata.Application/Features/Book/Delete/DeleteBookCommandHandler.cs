using Kata.Application.Features.Reservation;
using Kata.Domain.Book;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Book.Delete;

internal sealed class DeleteBookCommandHandler(
    IBookWriteRepository bookWriteRepository,
    IReservationReadRepository reservationReadRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        Domain.Book.Book? book = await bookWriteRepository.GetById(command.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound);
        }

        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByBooks([book.Id], cancellationToken);

        // Taking a held book out of the catalogue would leave a reader waiting for something
        // that no longer exists, so the refusal is explicit rather than silent.
        if (open.Any(reservation => reservation.IsActive(dateTimeProvider.UtcNow)))
        {
            return Result.Failure(BookErrors.HasActiveReservation);
        }

        book.Delete();

        await bookWriteRepository.SaveChanges(cancellationToken);

        return Result.Success();
    }
}
