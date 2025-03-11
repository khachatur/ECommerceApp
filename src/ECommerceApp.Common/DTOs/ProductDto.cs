using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Common.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000")]
        public decimal Price { get; set; }

        [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10,000")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }   
}
