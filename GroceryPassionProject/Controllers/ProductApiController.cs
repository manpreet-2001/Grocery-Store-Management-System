using GroceryPassionProject.Data;
using GroceryPassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryPassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO for Product to avoid circular references and expose only needed fields
        public class ProductDto
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = null!;
            public decimal Price { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = null!;
        }

        // GET: api/ProductsApi/List
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> ListProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/ProductsApi/Find/5
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ProductDto>> FindProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            return Ok(product);
        }

        // PUT: api/ProductsApi/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
        {
            if (id != productDto.ProductId)
                return BadRequest("Product ID mismatch");

            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
                return NotFound("Category not found");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.ProductId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/ProductsApi/Add
        [HttpPost("Add")]
        public async Task<ActionResult<ProductDto>> AddProduct(ProductDto productDto)
        {
            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
                return NotFound("Category not found");

            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.ProductId = product.ProductId;
            productDto.CategoryName = category.Name;

            return CreatedAtAction(nameof(FindProduct), new { id = product.ProductId }, productDto);
        }

        // DELETE: api/ProductsApi/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
