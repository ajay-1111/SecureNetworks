namespace SecureNetworks.Models.DBModels
{
    public class SNUserCartEntity
    {
        public int SNCartId { get; set; }

        public int SNProductId { get; set; }

        public string SNUserId { get; set; }

        public int SNProductQuantity { get; set; }

        public double SNProductPrice { get; set; }
    }
}
