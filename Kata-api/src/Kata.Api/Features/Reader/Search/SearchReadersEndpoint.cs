using Kata.Application.Features.Reader;
using Kata.Application.Features.Reader.Search;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reader.Search;

internal sealed class SearchReadersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/readers", async (
                [FromQuery] string? name,
                [FromQuery] string? email,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new SearchReadersQuery(name, email), cancellationToken)
                    .Match(
                        Results.Ok,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Readers)
            .WithName("SearchReaders")
            .WithSummary("List the readers, optionally filtered by name or email")
            .WithDescription(
                "Both filters match on a fragment and ignore case. Each reader reports how many " +
                "books it is currently holding.")
            .Produces<IReadOnlyCollection<ReaderSummaryResponse>>(StatusCodes.Status200OK);
    }
}
