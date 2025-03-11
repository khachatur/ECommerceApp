using ECommerceApp.Common.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace ECommerceApp.WebApp.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public List<OrderDto> Orders { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var apiUrl = _configuration["ApiBaseUrl"];
            var token = HttpContext.Session.GetString("JwtToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{apiUrl}/api/orders");
            if (response.IsSuccessStatusCode)
            {
                var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
                if (orders != null)
                {
                    Orders = orders;
                }
            }
            else
            {
                _logger.LogError("Failed to fetch orders. Status code: {StatusCode}", response.StatusCode);
            }
        }
    }
}