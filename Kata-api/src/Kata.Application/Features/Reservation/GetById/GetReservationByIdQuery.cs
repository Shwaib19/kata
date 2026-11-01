using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.GetById;

public sealed record GetReservationByIdQuery(Guid ReservationId) : IQuery<ReservationResponse>;
