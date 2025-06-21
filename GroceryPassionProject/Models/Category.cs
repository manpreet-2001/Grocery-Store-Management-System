
using System.ComponentModel.DataAnnotations;

namespace GroceryPassionProject.Models

{

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        //One Category can have many products
        public List<Product> Products { get; set; }
    }

}
