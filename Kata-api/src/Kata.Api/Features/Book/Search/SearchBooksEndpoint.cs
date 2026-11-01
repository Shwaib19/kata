using Kata.Application.Features.Book;
using Kata.Application.Features.Book.Search;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Book.Search;

internal sealed class SearchBooksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/books", async (
                [FromQuery] string? title,
                [FromQuery] string? author,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new SearchBooksQuery(title, author), cancellationToken)
                    .Match(
                        Results.Ok,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Books)
            .WithName("SearchBooks")
            .WithSummary("List the catalogue, optionally filtered by title or author")
            .WithDescription(
                "Both filters match on a fragment and ignore case. Each book reports whether it is " +
                "available, which is derived from the reservations holding it right now.")
            .Produces<IReadOnlyCollection<BookResponse>>(StatusCodes.Status200OK);
    }
}
