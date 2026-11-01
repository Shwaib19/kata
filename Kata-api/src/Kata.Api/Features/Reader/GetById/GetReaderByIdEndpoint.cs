using Kata.Application.Features.Reader;
using Kata.Application.Features.Reader.GetById;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reader.GetById;

internal sealed class GetReaderByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/readers/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await sender.SendAsync(new GetReaderByIdQuery(id), cancellationToken)
                    .Match(
                        Results.Ok,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Readers)
            .WithName("GetReaderById")
            .WithSummary("Read one reader and the reservations currently holding a book")
            .Produces<ReaderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
