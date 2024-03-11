// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CartController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the CartController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Models.DBModels;
using SecureNetworks.Models.ViewModel;

namespace SecureNetworks.Controllers.Cart
{
    public class CartController : Controller
    {
        private readonly SecureNetworkDBContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<ApplicationUserDBEntity> _userManager;

        private readonly SignInManager<ApplicationUserDBEntity> _signInManager;

        public CartController(SecureNetworkDBContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUserDBEntity> userManager, SignInManager<ApplicationUserDBEntity> signInManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["CartItemCount"] = null;

            var cartItems = await GetCartItemsForCurrentUser();
            ViewBag.CartItemCount = await GetCartItemCount();

            // Set the cart item count in TempData
            int cartItemCount = await GetCartItemCount();
            TempData["CartItemCount"] = cartItemCount;

            return View(cartItems);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int id)
        {
            TempData["CartItemCount"] = null;

            // Retrieve product details from the database
            var product = await _context.tbl_SNProducts.FirstOrDefaultAsync(p => p.SNProductId == id);

            var checkIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            var user = _userManager.GetUserId(User);

            if (checkIfUserSignedInOrNot)
            {
                if (user != null)
                {
                    // Check if the item is already in the cart.
                    var getTheQuantity = await _context.tbl_SNUserCartEntities.FirstOrDefaultAsync(p => p.SNProductId == id);
                    if (getTheQuantity != null)
                    {
                        // If the item is already in the cart then increase the quantity.
                        getTheQuantity.SNProductQuantity += 1;

                        _context.Update(getTheQuantity);
                    }
                    else
                    {
                        // user has no cart but adding a new item to the existing cart.

                        if (product != null)
                        {
                            SNUserCartEntity newUserCartEntity = new SNUserCartEntity()
                            {
                                SNProductId = product.SNProductId,
                                SNUserId = user,
                                SNProductQuantity = 1,
                                SNProductPrice = product.SNProductPrice
                            };

                            await _context.tbl_SNUserCartEntities.AddAsync(newUserCartEntity);
                        }
                    }
                }
                else
                {
                    // user has no cart. Adding a brand new cart for the user.

                    if (product != null)
                    {
                        SNUserCartEntity newUserCartEntity = new SNUserCartEntity()
                        {
                            SNProductId = product.SNProductId,
                            SNUserId = user!,
                            SNProductQuantity = 1,
                            SNProductPrice = product.SNProductPrice
                        };

                        await _context.tbl_SNUserCartEntities.AddAsync(newUserCartEntity);
                    }
                    else
                    {
                        TempData["CartItemCount"] = $"No Products found for this user : {user} in the car";
                    }

                }

                await _context.SaveChangesAsync();
            }
            
            // Set the cart item count in TempData
            int cartItemCount = await GetCartItemCount();
            TempData["CartItemCount"] = cartItemCount;

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
        private async Task<List<SNCartItemViewModel>> GetCartItemsForCurrentUser()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var cartItems = await (from uc in _context.tbl_SNUserCartEntities
                                           join p in _context.tbl_SNProducts on uc.SNProductId equals p.SNProductId
                                           where uc.SNUserId == user.Id
                                           select new SNCartItemViewModel
                                           {
                                               SNProductId = p.SNProductId,
                                               ImageUrl = p.SNProductImageUrl,
                                               SNProductName = p.SNProductName,
                                               SNProductPrice = p.SNProductPrice,
                                               SNProductQuantity = uc.SNProductQuantity
                                           }).ToListAsync();

                    return cartItems;
                }
            }
            return new List<SNCartItemViewModel>();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            TempData["CartItemCount"] = "";

            // Retrieve the current user's ID from the HttpContext
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                // Handle the case where the user is not authenticated
                return Unauthorized();
            }

            // Find the item in the user's cart
            var cartItem = await _context.tbl_SNUserCartEntities.FirstOrDefaultAsync(c => c.SNProductId == productId && c.SNUserId == userId);

            if (cartItem != null)
            {
                _context.tbl_SNUserCartEntities.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Return a success response
                return Json(new { success = true });
            }

            // Set the cart item count in TempData
            int cartItemCount = await GetCartItemCount();
            TempData["CartItemCount"] = cartItemCount;

            // Return an error response if the item was not found in the cart
            return Json(new { success = false });
        }
    }
}
