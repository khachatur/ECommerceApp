using System.ComponentModel.DataAnnotations;

namespace ECommerceApp.Common.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required"), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
