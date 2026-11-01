using SharedKernel.Messaging;

namespace Kata.Application.Features.Reservation.Reserve;

public sealed record ReserveBookCommand(
    Guid BookId,
    Guid ReaderId) : ICommand<Guid>;
