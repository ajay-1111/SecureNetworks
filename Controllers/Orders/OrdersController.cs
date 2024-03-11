using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Models.DBModels;

namespace SecureNetworks.Controllers.Orders
{
    public class OrdersController : Controller
    {
        private readonly SecureNetworkDBContext _context;

        private readonly UserManager<ApplicationUserDBEntity> _userManager;

        private readonly SignInManager<ApplicationUserDBEntity> _signInManager;


        public OrdersController(SecureNetworkDBContext context, SignInManager<ApplicationUserDBEntity> signInManager, UserManager<ApplicationUserDBEntity> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var cartItems = _context.tbl_SNUserCartEntities.Where(c => c.SNUserId == user.Id).ToList();

                    if (cartItems.Any())
                    {
                        // Create an order
                        var order = new SNPurchaseOrderEntity()
                        {
                            SNUserId = user.Id,
                            SNOrderDate = DateTime.Now
                        };

                        _context.tbl_SNPurchaseOrderEntities.Add(order);
                        await _context.SaveChangesAsync();

                        // Create order items for each cart item
                        foreach (var cartItem in cartItems)
                        {
                            var orderItem = new SNPurchaseOrderItemEntity()
                            {
                                SNOrderId = order.SNOrderId,
                                SNProductId = cartItem.SNProductId,
                                Quantity = cartItem.SNProductQuantity,
                                Price = cartItem.SNProductPrice
                            };

                            _context.tbl_SNPurchaseOrderItemEntities.Add(orderItem);
                        }

                        // Remove the cart items associated with this user
                        _context.tbl_SNUserCartEntities.RemoveRange(cartItems);
                        await _context.SaveChangesAsync();

                        int cartItemCount = await GetCartItemCount();
                        TempData["CartItemCount"] = cartItemCount;

                        return Json(new { success = true, message = "Order placed successfully!" });
                    }

                    return Json(new { success = false, message = "Your cart is empty. Please add items before placing an order." });
                }

                return Json(new { success = false, message = "User not found. Please sign in to place an order." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to place order. Please try again later." });
            }
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
    }
}
