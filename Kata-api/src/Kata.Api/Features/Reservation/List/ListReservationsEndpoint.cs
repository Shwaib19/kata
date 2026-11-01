using Kata.Application.Features.Reservation;
using Kata.Application.Features.Reservation.List;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reservation.List;

internal sealed class ListReservationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/reservations", async (
                [FromQuery] Guid? readerId,
                [FromQuery] Guid? bookId,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new ListReservationsQuery(readerId, bookId), cancellationToken)
                    .Match(
                        Results.Ok,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Reservations)
            .WithName("ListReservations")
            .WithSummary("List the reservations, most recent first")
            .WithDescription(
                "Filter by reader, by book, or by both. Includes the reservations that are over, " +
                "and each one reports the status observed now, so an abandoned reservation reads " +
                "as expired.")
            .Produces<IReadOnlyCollection<ReservationResponse>>(StatusCodes.Status200OK);
    }
}
