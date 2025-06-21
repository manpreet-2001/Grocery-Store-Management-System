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

        public class ProductDto
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = null!;
            public decimal Price { get; set; }
            public int CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? ImageUrl { get; set; }
            public List<string>? SupplierNames { get; set; }

            public List<int>? SupplierIds { get; set; }
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> ListProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.ImageUrl,
                    SupplierNames = p.ProductSuppliers
                        .Select(ps => ps.Supplier.Name)
                        .ToList()
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ProductDto>> FindProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.ImageUrl,
                    SupplierNames = p.ProductSuppliers
                        .Select(ps => ps.Supplier.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
        {
            if (id != productDto.ProductId)
                return BadRequest("Product ID mismatch");

            var category = await _context.Categories.FindAsync(productDto.CategoryId);
            if (category == null)
                return NotFound("Category not found");

            var product = await _context.Products
                .Include(p => p.ProductSuppliers) // Include current suppliers
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            // Update basic fields
            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.CategoryId = productDto.CategoryId;
            product.ImageUrl = productDto.ImageUrl;

            // ✅ Update suppliers
            if (productDto.SupplierIds != null)
            {
                // Remove existing relationships
                product.ProductSuppliers.Clear();

                // Add new relationships
                foreach (var supplierId in productDto.SupplierIds)
                {
                    bool supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == supplierId);
                    if (!supplierExists)
                        return NotFound($"Supplier ID {supplierId} not found");

                    product.ProductSuppliers.Add(new ProductSupplier
                    {
                        ProductId = product.ProductId,
                        SupplierId = supplierId
                    });
                }
            }

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
                CategoryId = productDto.CategoryId,
                ImageUrl = productDto.ImageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.ProductId = product.ProductId;
            productDto.CategoryName = category.Name;

            return CreatedAtAction(nameof(FindProduct), new { id = product.ProductId }, productDto);
        }

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
