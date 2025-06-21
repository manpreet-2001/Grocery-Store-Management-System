using System.Security.Claims;
using GroceryPassionProject.Data;
using GroceryPassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace GroceryPassionProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // This line restricts access to authenticated users only
    public class CartApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class AddToCartDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("USER ID: " + userId);

                return Unauthorized(); // User must be logged in
            }

            Console.WriteLine("USER ID: " + userId); // This will print only if user is logged in
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
