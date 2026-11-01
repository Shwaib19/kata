using Kata.Application.Features.Reader.Delete;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reader.Delete;

internal sealed class DeleteReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/readers/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new DeleteReaderCommand(id), cancellationToken)
                    .Match(
                        Results.NoContent,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Readers)
            .WithName("DeleteReader")
            .WithSummary("Remove a reader")
            .WithDescription(
                "The row is kept so that past reservations still point at something. " +
                "A reader still holding a book is refused.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
