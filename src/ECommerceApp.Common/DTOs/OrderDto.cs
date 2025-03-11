namespace ECommerceApp.Common.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}
