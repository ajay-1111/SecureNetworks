using System.ComponentModel.DataAnnotations;

namespace SecureNetworks.Models.DBModels
{
    public class SNUserCartEntity
    {
        [Key]
        public int SNCartId { get; set; }

        public int SNProductId { get; set; }

        public string SNUserId { get; set; }

        public int SNProductQuantity { get; set; }

        public double SNProductPrice { get; set; }
    }
}
