using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNetworks.Models.DBModels
{
    public class SNPurchaseOrderItemEntity
    {
        [Key]
        public int Id { get; set; } // Primary key

        [ForeignKey("SNOrderId")]
        public int SNOrderId { get; set; } // Foreign key to the order
        
        public int SNProductId { get; set; } // Foreign key to the product

        public int Quantity { get; set; }

        public double Price { get; set; }

        // Navigation properties
        public SNPurchaseOrderEntity Order { get; set; }

        public SNProductsEntity Product { get; set; }
    }
}
