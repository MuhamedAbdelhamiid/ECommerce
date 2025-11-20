using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ECommerce.Shared.DTOs.BasketDTOs
{
    public class BasketItemDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string PictureUrl { get; set; } = default!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public int Quantity { get; set; }
    }
}
