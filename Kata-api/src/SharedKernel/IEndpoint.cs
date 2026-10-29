using Microsoft.AspNetCore.Routing;

namespace SharedKernel;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
