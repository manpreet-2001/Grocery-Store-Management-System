using System.ComponentModel.DataAnnotations;

namespace GroceryPassionProject.Models
{

    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Phone]
        public string Contact { get; set; }

        public List<ProductSupplier> ProductSuppliers { get; set; }
    }
}
