using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Borrow;

public sealed record BorrowReservationCommand(Guid ReservationId) : ICommand;
