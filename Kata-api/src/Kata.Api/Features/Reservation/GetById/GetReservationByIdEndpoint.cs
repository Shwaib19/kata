using Kata.Application.Features.Reservation;
using Kata.Application.Features.Reservation.GetById;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reservation.GetById;

internal sealed class GetReservationByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/reservations/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new GetReservationByIdQuery(id), cancellationToken)
                    .Match(
                        Results.Ok,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Reservations)
            .WithName("GetReservationById")
            .WithSummary("Read one reservation")
            .Produces<ReservationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
