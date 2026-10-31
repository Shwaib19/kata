using Kata.Application.Features.Book.Create;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Book.Create;

internal sealed record CreateBookRequest(string? Title, string? Author, int? PublicationYear);

internal sealed class CreateBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/books", async (
                [FromBody] CreateBookRequest request,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await Result.Create(request)
                    .Map(_ => new CreateBookCommand(request.Title, request.Author, request.PublicationYear))
                    .Bind(command => sender.SendAsync(command, cancellationToken))
                    .Match(
                        id => Results.CreatedAtRoute("GetBookById", new { id }, id),
                        CustomResults.Problem);
            })
            .WithTags(Tags.Books)
            .WithName("CreateBook")
            .WithSummary("Add a book to the catalogue")
            .WithDescription("Title is required and capped at 50 characters; the publication year cannot be in the future.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
