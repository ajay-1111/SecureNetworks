using System.ComponentModel.DataAnnotations;

namespace SecureNetworks.Models.DBModels
{
    public class SNProductsEntity
    {
        public enum ProductCategory
        {
            NetworkDevices = 1,
            TrainingCourses = 2,
            TestingTools = 3,
        }

        [Key]
        public int SNProductId { get; set; }

        public ProductCategory SNProductCategory { get; set; }

        public string? SNProductName { get; set; }

        public double SNProductPrice { get; set; }

        public double SNProductRating { get; set; }

        public string? SNProductDescription { get; set; }

        public int SNProductStock { get; set; }

        public string? SNProductImageUrl { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime ModifieDateTime { get; set; }
    }
}
