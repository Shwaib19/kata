using Kata.Application.Features.Book.Delete;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Book.Delete;

internal sealed class DeleteBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/books/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new DeleteBookCommand(id), cancellationToken)
                    .Match(
                        Results.NoContent,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Books)
            .WithName("DeleteBook")
            .WithSummary("Take a book out of the catalogue")
            .WithDescription(
                "The row is kept so that past reservations still point at something. " +
                "A book currently reserved or borrowed is refused.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
