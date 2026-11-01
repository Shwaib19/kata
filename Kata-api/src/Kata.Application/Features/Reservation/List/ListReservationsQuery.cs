using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.List;

/// <summary>
/// Lists every reservation, past and present. Passing a reader narrows it to that reader's history.
/// </summary>
public sealed record ListReservationsQuery(
    Guid? ReaderId) : IQuery<IReadOnlyCollection<ReservationResponse>>;
