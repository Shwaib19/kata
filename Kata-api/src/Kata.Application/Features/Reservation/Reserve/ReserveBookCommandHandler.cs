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

        // Reading a reservation derives its expiry, but the uniqueness guarantee is built on the
        // stored status: a lapsed reservation left as Reserved would hold its book for good.
        // This is the one place that writes Expired, and it must run before the insert.
        await reservationWriteRepository.ExpireLapsed(
            command.BookId,
            utcNow - Domain.Reservation.Reservation.ExpiryWindow,
            cancellationToken);

        Result<Domain.Reservation.Reservation> reservation =
            Domain.Reservation.Reservation.Create(command.BookId, command.ReaderId, dateTimeProvider);

        if (reservation.IsFailure)
        {
            return Result.Failure<Guid>(reservation.Error);
        }

        await reservationWriteRepository.Add(reservation.Value, cancellationToken);

        // The check above can be outrun by another request reserving the same book. The store
        // is the only thing able to settle that, so its refusal is returned as it stands.
        Result saved = await reservationWriteRepository.SaveChanges(cancellationToken);

        return saved.IsFailure
            ? Result.Failure<Guid>(saved.Error)
            : Result.Success(reservation.Value.Id);
    }
}
