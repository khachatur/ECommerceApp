using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ECommerceApp.WebApp.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public OrderDto? Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/orders");
            if (response.IsSuccessStatusCode)
            {
                var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
                if (orders == null)
                {
                    return NotFound();
                }
                Order = orders.FirstOrDefault(o => o.Id == id);
                if (Order == null) return NotFound();
                return Page();
            }
            return NotFound();
        }
    }
}