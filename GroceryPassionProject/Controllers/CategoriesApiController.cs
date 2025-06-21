using GroceryPassionProject.Data;
using GroceryPassionProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace GroceryPassionProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize(Roles = "Admin")]
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

        /// <summary>
        /// Retrieves a list of all categories available in the system.
        /// </summary>
        /// <returns>A list of all categories.</returns>
        /// <example>GET: /api/CategoriesApi/List</example>
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

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to find.</param>
        /// <returns>A single category object.</returns>
        /// <example>GET: /api/CategoriesApi/Find/2</example>
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

        /// <summary>
        /// Updates the name of an existing category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="categoryDto">The updated category data.</param>
        /// <returns>No content if update is successful.</returns>
        /// <example>
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

        /// <summary>
        /// Adds a new category to the system.
        /// </summary>
        /// <param name="categoryDto">The category data to add.</param>
        /// <returns>The newly created category.</returns>
        /// <example>
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
        /// <summary>
        /// Deletes a category from the system by ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>No content if delete is successful.</returns>
        /// <example>DELETE: /api/CategoriesApi/Delete/4</example>
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
