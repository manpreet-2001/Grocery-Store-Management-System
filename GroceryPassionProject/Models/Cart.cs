using System.ComponentModel.DataAnnotations;

namespace GroceryPassionProject.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
