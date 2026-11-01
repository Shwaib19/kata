using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.List;

/// <summary>
/// Lists every reservation, past and present. A reader gives that reader's history, a book
/// gives the book's, and both together give the history between the two.
/// </summary>
public sealed record ListReservationsQuery(
    Guid? ReaderId,
    Guid? BookId) : IQuery<IReadOnlyCollection<ReservationResponse>>;
