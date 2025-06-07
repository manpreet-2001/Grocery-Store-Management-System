using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryPassionProject.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
       
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public List<ProductSupplier> ProductSuppliers { get; set; }
    }
        

    
}
