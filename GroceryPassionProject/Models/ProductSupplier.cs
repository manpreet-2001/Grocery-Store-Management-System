using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryPassionProject.Models
{

    public class ProductSupplier
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // One Product have supply many Products
        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}
