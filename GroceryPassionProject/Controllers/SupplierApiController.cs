using GroceryPassionProject.Data;
using GroceryPassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryPassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuppliersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO for Supplier
        public class SupplierDto
        {
            public int SupplierId { get; set; }
            public string Name { get; set; } = null!;
            public string Contact { get; set; } = null!;
        }

        // GET: api/SuppliersApi/List
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> ListSuppliers()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new SupplierDto
                {
                    SupplierId = s.SupplierId,
                    Name = s.Name,
                    Contact = s.Contact
                })
                .ToListAsync();

            return Ok(suppliers);
        }

        // GET: api/SuppliersApi/Find/5
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<SupplierDto>> FindSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Where(s => s.SupplierId == id)
                .Select(s => new SupplierDto
                {
                    SupplierId = s.SupplierId,
                    Name = s.Name,
                    Contact = s.Contact
                })
                .FirstOrDefaultAsync();

            if (supplier == null) return NotFound();

            return Ok(supplier);
        }

        // PUT: api/SuppliersApi/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, SupplierDto supplierDto)
        {
            if (id != supplierDto.SupplierId)
                return BadRequest("Supplier ID mismatch");

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            supplier.Name = supplierDto.Name;
            supplier.Contact = supplierDto.Contact;

            _context.Entry(supplier).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Suppliers.Any(s => s.SupplierId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/SuppliersApi/Add
        [HttpPost("Add")]
        public async Task<ActionResult<SupplierDto>> AddSupplier(SupplierDto supplierDto)
        {
            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                Contact = supplierDto.Contact
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            supplierDto.SupplierId = supplier.SupplierId;

            return CreatedAtAction(nameof(FindSupplier), new { id = supplier.SupplierId }, supplierDto);
        }

        // DELETE: api/SuppliersApi/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
