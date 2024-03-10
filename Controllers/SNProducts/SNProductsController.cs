// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SNProductsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the SNProductsController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Models.DBModels;
using SecureNetworks.Models.ViewModel;

namespace SecureNetworks.Controllers.SNProducts
{
    public class SNProductsController : Controller
    {
        private readonly SecureNetworkDBContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<ApplicationUserDBEntity> _userManager;

        private readonly SignInManager<ApplicationUserDBEntity> _signInManager;

        public SNProductsController(SecureNetworkDBContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUserDBEntity> userManager, SignInManager<ApplicationUserDBEntity> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Products()
        {
            TempData["CartItemCount"] = null;
            return View(new SNProductViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["NoProducts"] = null;
            TempData["CartItemCount"] = null;
            // Retrieve all products from the database
            var products = _context.tbl_SNProducts.ToList();

            // Check if products list is empty
            if (products.Count == 0)
            {
                // If products list is empty, set an error message using ViewBag
                TempData["NoProducts"] = "No products available at the moment.";
                return RedirectToAction("Products","SNProducts");
            }

            // Create a list to hold the view models for all products
            List<SNProductViewModel> productViewModels = new List<SNProductViewModel>();

            // Loop through each product and create a view model for it
            foreach (var product in products)
            {
                SNProductViewModel productModel = new SNProductViewModel()
                {
                    ImageUrl = product.SNProductImageUrl,
                    Name = product.SNProductName,
                    Price = product.SNProductPrice,
                    Rating = product.SNProductRating,
                    Stock = product.SNProductStock,
                    Id = product.SNProductId,
                };

                // Add the view model to the list
                productViewModels.Add(productModel);
            }

            int cartItemCount = await GetCartItemCount();
            TempData["CartItemCount"] = cartItemCount;

            // Pass the list of view models to the view
            return View(productViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> SNProductDetails(int productId)
        {
            TempData["NoProductFound"] = null;
            TempData["CartItemCount"] = null;

            // Retrieve the product with the specified id from the database
            var product = await _context.tbl_SNProducts.FirstOrDefaultAsync(p => p.SNProductId == productId);

            // Check if product is null
            if (product == null)
            {
                TempData["NoProductFound"] = $"Unable to find the product details for ID : {productId}";
            }

            if (product != null)
            {
                SNProductViewModel productModel = new SNProductViewModel()
                {
                    ImageUrl = product.SNProductImageUrl,
                    Name = product.SNProductName,
                    Price = product.SNProductPrice,
                    Rating = product.SNProductRating,
                    Stock = product.SNProductStock,
                    Description = product.SNProductDescription,
                    Id = product.SNProductId,
                };

                int cartItemCount = await GetCartItemCount();
                TempData["CartItemCount"] = cartItemCount;

                // Pass the product to the view for rendering
                return View(productModel);
            }

            TempData["NoProductFound"] = $"Unable to find the product details for ID : {productId}";

            return RedirectToAction("Index", "SNProducts");
        }

        private async Task<int> GetCartItemCount()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    return await _context.tbl_SNUserCartEntities
                        .Where(u => u.SNUserId == user.Id)
                        .SumAsync(u => u.SNProductQuantity);
                }
            }
            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            TempData["CartItemCount"] = null;

            if (!string.IsNullOrWhiteSpace(category))
            {
                var categoryEnum = (SNProductsEntity.ProductCategory)Enum.Parse(typeof(SNProductsEntity.ProductCategory), category, true);

                var products = await _context.tbl_SNProducts
                    .Where(p => p.SNProductCategory == categoryEnum)
                    .ToListAsync();

                if (products.Count == 0)
                {
                    TempData["NoProducts"] = $"No products available for category: {category}";
                    return RedirectToAction("Products", "SNProducts");
                }

                var productViewModels = products.Select(product => new SNProductViewModel()
                {
                    ImageUrl = product.SNProductImageUrl,
                    Name = product.SNProductName,
                    Price = product.SNProductPrice,
                    Rating = product.SNProductRating,
                    Stock = product.SNProductStock,
                    Id = product.SNProductId,
                }).ToList();

                int cartItemCount = await GetCartItemCount();
                TempData["CartItemCount"] = cartItemCount;

                return View("Index", productViewModels);
            }

            return RedirectToAction("Index");
        }
    }
}
