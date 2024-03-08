
namespace SecureNetworks.Models.ViewModel
{
    public class SNCartItemViewModel
    {
        public int SNProductId { get; set; }

        public string SNProductDescription { get; set; }

        public string ImageUrl { get; set; }

        public string SNProductName { get; set; }

        public double SNProductPrice { get; set; }

        public int SNProductQuantity { get; set; }

        public static implicit operator SNCartItemViewModel(SNProductViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}
