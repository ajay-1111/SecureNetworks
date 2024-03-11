using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Models.DBModels;

namespace SecureNetworks.Controllers.AboutUS
{
    public class AboutUsController : Controller
    {
        private readonly SecureNetworkDBContext _context;

        private readonly UserManager<ApplicationUserDBEntity> _userManager;

        private readonly SignInManager<ApplicationUserDBEntity> _signInManager;

        public AboutUsController(SecureNetworkDBContext context, UserManager<ApplicationUserDBEntity> userManager, SignInManager<ApplicationUserDBEntity> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            TempData["CartItemCount"] = null;
            
            ViewBag.CartItemCount = await GetCartItemCount();

            return View();
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
