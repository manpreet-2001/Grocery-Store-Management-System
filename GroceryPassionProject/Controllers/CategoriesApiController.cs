using GroceryPassionProject.Data;
using GroceryPassionProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryPassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DTO for Category
        public class CategoryDto
        {
            public int CategoryId { get; set; }
            public string Name { get; set; } = null!;
        }

        // GET: api/CategoriesApi/List
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> ListCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/CategoriesApi/Find/5
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<CategoryDto>> FindCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();

            if (category == null) return NotFound();

            return Ok(category);
        }

        // PUT: api/CategoriesApi/Update/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto categoryDto)
        {
            if (id != categoryDto.CategoryId)
                return BadRequest("Category ID mismatch");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = categoryDto.Name;
            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(c => c.CategoryId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/CategoriesApi/Add
        [HttpPost("Add")]
        public async Task<ActionResult<CategoryDto>> AddCategory(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.CategoryId = category.CategoryId;

            return CreatedAtAction(nameof(FindCategory), new { id = category.CategoryId }, categoryDto);
        }

        // DELETE: api/CategoriesApi/Delete/5
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
