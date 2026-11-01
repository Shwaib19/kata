using Kata.Application.Features.Reservation.Return;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reservation.Return;

internal sealed class ReturnReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/reservations/{id:guid}/return", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new ReturnReservationCommand(id), cancellationToken)
                    .Match(
                        Results.NoContent,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Reservations)
            .WithName("ReturnReservation")
            .WithSummary("Bring the book back")
            .WithDescription("Releases the book. Only a borrowed book can be returned.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
