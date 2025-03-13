namespace ECommerceApp.WebApp.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetCurrentUserAsync(HttpContext httpContext);
        Task<List<UserInfo>> GetUsersAsync(string adminToken);
    }
}
