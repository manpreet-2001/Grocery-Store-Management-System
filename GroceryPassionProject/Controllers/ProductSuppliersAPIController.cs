using GroceryPassionProject.Data;
using GroceryPassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryPassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSuppliersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductSuppliersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO for ProductSupplier
        public class ProductSupplierDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = null!;
            public int SupplierId { get; set; }
            public string SupplierName { get; set; } = null!;
        }

        // GET: api/ProductSuppliersApi/List
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ProductSupplierDto>>> ListProductSuppliers()
        {
            var list = await _context.ProductSuppliers
                .Include(ps => ps.Product)
                .Include(ps => ps.Supplier)
                .Select(ps => new ProductSupplierDto
                {
                    ProductId = ps.ProductId,
                    ProductName = ps.Product.Name,
                    SupplierId = ps.SupplierId,
                    SupplierName = ps.Supplier.Name
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST: api/ProductSuppliersApi/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddProductSupplier(ProductSupplierDto dto)
        {
            // Validate existence of Product and Supplier
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);
            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == dto.SupplierId);

            if (!productExists) return NotFound("Product not found");
            if (!supplierExists) return NotFound("Supplier not found");

            var exists = await _context.ProductSuppliers.AnyAsync(ps => ps.ProductId == dto.ProductId && ps.SupplierId == dto.SupplierId);
            if (exists) return Conflict("This ProductSupplier already exists");

            var productSupplier = new ProductSupplier
            {
                ProductId = dto.ProductId,
                SupplierId = dto.SupplierId
            };

            _context.ProductSuppliers.Add(productSupplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListProductSuppliers), null);
        }

        // DELETE: api/ProductSuppliersApi/Delete/5/7
        [HttpDelete("Delete/{productId}/{supplierId}")]
        public async Task<IActionResult> DeleteProductSupplier(int productId, int supplierId)
        {
            var productSupplier = await _context.ProductSuppliers
                .FindAsync(productId, supplierId);

            if (productSupplier == null) return NotFound();

            _context.ProductSuppliers.Remove(productSupplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
