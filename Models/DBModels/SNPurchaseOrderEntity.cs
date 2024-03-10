using System.ComponentModel.DataAnnotations;

namespace SecureNetworks.Models.DBModels
{
    public class SNPurchaseOrderEntity
    {
        [Key]
        public int SNOrderId { get; set; }

        public string? SNUserId { get; set; }

        public DateTime SNOrderDate { get; set; }
    }
}
