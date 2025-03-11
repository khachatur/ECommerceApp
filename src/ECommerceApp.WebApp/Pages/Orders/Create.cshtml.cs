using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerceApp.WebApp.Pages.Orders;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public CreateModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [BindProperty]
    public List<OrderItemDto> OrderItems { get; set; } = new();

    public List<ProductDto> Products { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/products");
        if (response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
            if (products != null)
            {
                Products = products;
                OrderItems = Products.Select(p => new OrderItemDto { ProductId = p.Id }).ToList();
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var client = _clientFactory.CreateClient("ApiClient");
        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var orderRequest = new { OrderItems = OrderItems.Where(oi => oi.Quantity > 0).ToList() };
        var response = await client.PostAsJsonAsync("api/orders", orderRequest);
        if (response.IsSuccessStatusCode)
            return RedirectToPage("/Orders/Index");

        ModelState.AddModelError("", "Failed to create order.");
        await OnGetAsync();
        return Page();
    }
}

    