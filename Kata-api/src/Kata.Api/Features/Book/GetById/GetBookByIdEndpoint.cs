using Kata.Application.Features.Book;
using Kata.Application.Features.Book.GetById;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Book.GetById;

internal sealed class GetBookByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/books/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new GetBookByIdQuery(id), cancellationToken)
                    .Match(
                        Results.Ok,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Books)
            .WithName("GetBookById")
            .WithSummary("Read one book")
            .Produces<BookResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
