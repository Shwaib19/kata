using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Cancel;

public sealed record CancelReservationCommand(Guid ReservationId) : ICommand;
