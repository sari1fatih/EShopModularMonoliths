using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace Catalog.Products.Features.GetProducts;

//public record GetProductsRequest(); 
public record GetProductsResponse(PaginatedResult<ProductDto> Products);

public class GetProductsEndpoint : ControllerBase //4: ICarterModule
{
    private readonly ISender _sender;
    public GetProductsEndpoint(ISender sender)
    {
        _sender = sender;
    }
    [HttpGet("/products")]
    [ProducesResponseType(typeof(GetProductsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ActionName("GetProducts")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    [Tags("Get Products")]
    public async Task<IActionResult> GetHello([AsParameters] PaginationRequest request)
    {
        var result = await _sender.Send(new GetProductsQuery(request));

        var response = result.Adapt<GetProductsResponse>();

        return Ok(response);
    }
    /*public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (
                [AsParameters] PaginationRequest request,
                ISender sender) =>
            {
                var result = await sender.Send(new GetProductsQuery(request));

                var response = result.Adapt<GetProductsResponse>();

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
    }*/
}