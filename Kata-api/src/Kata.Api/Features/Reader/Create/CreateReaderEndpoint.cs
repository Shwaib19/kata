using Kata.Application.Features.Reader.Create;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using SharedKernel.Messaging;

namespace Kata.Api.Features.Reader.Create;

internal sealed record CreateReaderRequest(string? Name, string? Email);

internal sealed class CreateReaderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/readers", async (
                [FromBody] CreateReaderRequest request,
                [FromServices] ISender sender,
                CancellationToken cancellationToken) =>
            {
                return await Result.Create(request)
                    .Map(_ => new CreateReaderCommand(request.Name, request.Email))
                    .Bind(command => sender.SendAsync(command, cancellationToken))
                    .Match(
                        id => Results.CreatedAtRoute("GetReaderById", new { id }, id),
                        CustomResults.Problem);
            })
            .WithTags(Tags.Readers)
            .WithName("CreateReader")
            .WithSummary("Register a reader")
            .WithDescription("The email is stored trimmed and lowercased, and must not already belong to a reader.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}
