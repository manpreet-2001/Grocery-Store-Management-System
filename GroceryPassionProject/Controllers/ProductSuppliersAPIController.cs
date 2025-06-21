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

        /// <summary>
        /// A DTO that represents the link between a Product and a Supplier.
        /// </summary>
        public class ProductSupplierDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = null!;
            public int SupplierId { get; set; }
            public string SupplierName { get; set; } = null!;
        }

        /// <summary>
        /// Retrieves a list of all product-supplier relationships from the system.
        /// </summary>
        /// <returns>
        /// A list of ProductSupplierDto objects including product and supplier names and their IDs.
        /// </returns>

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
        /// <summary>
        /// Creates a new product-supplier relationship if both entities exist and are not already linked.
        /// </summary>
        /// <param name="dto">An object containing the ProductId and SupplierId to link.</param>
        /// <returns>
        /// 201 Created if successfully added, 409 Conflict if the link already exists, 
        /// or 404 Not Found if the Product or Supplier does not exist.
        /// </returns>
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
        /// <summary>
        /// Deletes a specific product-supplier relationship based on composite key.
        /// </summary>
        /// <param name="productId">ID of the Product.</param>
        /// <param name="supplierId">ID of the Supplier.</param>
        /// <returns>
        /// 204 No Content if deleted, or 404 Not Found if no such link exists.
        /// </returns>
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
