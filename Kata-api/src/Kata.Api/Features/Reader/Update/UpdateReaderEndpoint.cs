using Kata.Application.Features.Reader.Update;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reader.Update;

internal sealed record UpdateReaderRequest(string? Name, string? Email);

internal sealed class UpdateReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/readers/{id:guid}", async (
                [FromRoute] Guid id,
                [FromBody] UpdateReaderRequest request,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await Result.Create(request)
                    .Map(_ => new UpdateReaderCommand(id, request.Name, request.Email))
                    .Bind(command => sender.SendAsync(command, cancellationToken))
                    .Match(
                        Results.NoContent,
                        CustomResults.Problem);
            })
            .WithTags(Tags.Readers)
            .WithName("UpdateReader")
            .WithSummary("Replace a reader's details")
            .WithDescription("The email must not already belong to another reader.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
