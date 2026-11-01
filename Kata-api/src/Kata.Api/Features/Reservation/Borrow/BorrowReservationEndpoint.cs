using Kata.Application.Features.Reservation.Borrow;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reservation.Borrow;

internal sealed class BorrowReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/reservations/{id:guid}/borrow", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new BorrowReservationCommand(id), cancellationToken)
                    .Match(
                        Results.NoContent,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Reservations)
            .WithName("BorrowReservation")
            .WithSummary("Pick the book up")
            .WithDescription("Turns the reservation into a loan. A reservation older than 48 hours has expired and is refused.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
