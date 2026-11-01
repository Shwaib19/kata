using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Return;

public sealed record ReturnReservationCommand(Guid ReservationId) : ICommand;
