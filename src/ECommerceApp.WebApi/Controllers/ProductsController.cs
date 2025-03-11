using ECommerceApp.Application.Features.Products.Commands;
using ECommerceApp.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var product = await _mediator.Send(new GetProductQuery { Id = id });
            return Ok(product);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id = productId }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        command.Id = id;
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            await _mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}