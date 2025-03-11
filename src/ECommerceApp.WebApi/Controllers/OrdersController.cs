using ECommerceApp.Application.Features.Orders.Commands;
using ECommerceApp.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApp.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        command.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orderId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrders), new { userId = command.UserId }, orderId);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _mediator.Send(new GetOrdersQuery { UserId = userId });
        return Ok(orders);
    }
}
