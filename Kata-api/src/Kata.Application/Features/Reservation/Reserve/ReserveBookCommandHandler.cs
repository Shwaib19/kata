using Kata.Application.Features.Book;
using Kata.Application.Features.Reader;
using Kata.Domain.Book;
using Kata.Domain.Reader;
using Kata.Domain.Reservation;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Reserve;

internal sealed class ReserveBookCommandHandler(
    IBookReadRepository bookReadRepository,
    IReaderReadRepository readerReadRepository,
    IReservationReadRepository reservationReadRepository,
    IReservationWriteRepository reservationWriteRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ReserveBookCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ReserveBookCommand command, CancellationToken cancellationToken)
    {
        Domain.Book.Book? book = await bookReadRepository.GetById(command.BookId, cancellationToken);

        if (book is null)
        {
            return Result.Failure<Guid>(BookErrors.NotFound);
        }

        if (!await readerReadRepository.Exists(command.ReaderId, cancellationToken))
        {
            return Result.Failure<Guid>(ReaderErrors.NotFound);
        }

        DateTime utcNow = dateTimeProvider.UtcNow;

        // The reader's open reservations are few, so they are counted in memory rather than in
        // SQL: expiry depends on elapsed time, and only the domain knows that rule.
        IReadOnlyCollection<Domain.Reservation.Reservation> open =
            await reservationReadRepository.GetOpenByReader(command.ReaderId, cancellationToken);

        if (open.Count(reservation => reservation.IsActive(utcNow)) >=
            Domain.Reservation.Reservation.MaxActivePerReader)
        {
            return Result.Failure<Guid>(ReservationErrors.QuotaExceeded);
        }

        Result<Domain.Reservation.Reservation> reservation =
            Domain.Reservation.Reservation.Create(command.BookId, command.ReaderId, dateTimeProvider);

        if (reservation.IsFailure)
        {
            return Result.Failure<Guid>(reservation.Error);
        }

        await reservationWriteRepository.Add(reservation.Value, cancellationToken);
        await reservationWriteRepository.SaveChanges(cancellationToken);

        return Result.Success(reservation.Value.Id);
    }
}
