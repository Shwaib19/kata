using Kata.Application.Features.Reservation.Reserve;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reservation.Reserve;

internal sealed record ReserveBookRequest(Guid BookId, Guid ReaderId);

internal sealed class ReserveBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/reservations", async (
                [FromBody] ReserveBookRequest request,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await Result.Create(request)
                    .Map(_ => new ReserveBookCommand(request.BookId, request.ReaderId))
                    .Bind(command => sender.SendAsync(command, cancellationToken))
                    .Match(
                        id => Results.CreatedAtRoute("GetReservationById", new { id }, id),
                        CustomResults.Problem);
            })
            .WithTags(Tags.Reservations)
            .WithName("ReserveBook")
            .WithSummary("Reserve a book")
            .WithDescription(
                "Holds a book for a reader for 48 hours. A reader may hold at most 3 reservations at once, " +
                "and a book already reserved or borrowed cannot be reserved again.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
