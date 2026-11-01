using Kata.Application.Features.Book.Update;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Book.Update;

internal sealed record UpdateBookRequest(string? Title, string? Author, int? PublicationYear);

internal sealed class UpdateBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/books/{id:guid}", async (
                [FromRoute] Guid id,
                [FromBody] UpdateBookRequest request,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await Result.Create(request)
                    .Map(_ => new UpdateBookCommand(id, request.Title, request.Author, request.PublicationYear))
                    .Bind(command => sender.SendAsync(command, cancellationToken))
                    .Match(
                        Results.NoContent,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Books)
            .WithName("UpdateBook")
            .WithSummary("Replace a book's details")
            .WithDescription("The same rules as creation apply, and a rejected update leaves the book unchanged.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
