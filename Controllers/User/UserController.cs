// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Helpers;
using SecureNetworks.Models.DBModels;
using SecureNetworks.Models.ViewModel;

namespace SecureNetworks.Controllers.User
{
    public class UserController : Controller
    {
        private readonly SecureNetworkDBContext _secureNetworkDbContext;

        private readonly SignInManager<ApplicationUserDBEntity> signInManager;
        
        private readonly UserManager<ApplicationUserDBEntity> userManager;

        private readonly UserServiceHepler _userServiceHepler;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpContextAccessor _httpContextAccessor ,UserServiceHepler _userServiceHepler,SecureNetworkDBContext _secureNetworkDbContext, SignInManager<ApplicationUserDBEntity> signInManager, UserManager<ApplicationUserDBEntity> userManager)
        {
            this._httpContextAccessor = _httpContextAccessor;
            this._userServiceHepler = _userServiceHepler;
            this._secureNetworkDbContext = _secureNetworkDbContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            TempData["LoginErrorMessage"] = "";

            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Retrieve the user from the database
                    var user = await _secureNetworkDbContext.AspNetUsers.FirstOrDefaultAsync(p => p.Email == model.Email);

                    // Check if the user is found and if they have the IsAdminUser flag set
                    _httpContextAccessor.HttpContext?.Session.SetInt32("IsAdminUser", user!.IsAdminUser == null ? 0 : 1);

                    return RedirectToAction("Index", "SNProducts");
                }

                TempData["LoginError"] = "Please use valid username/password, Contact admin incase of issues.";
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            TempData["RegisterationMessage"] = "";
            TempData["RegisterationErrorMessage"] = "";

            if (ModelState.IsValid)
            {
                var errorMessage = string.Empty;

                var newUser = new ApplicationUserDBEntity()
                {
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Telephone = model.Telephone,
                    IsAdminUser = 0
                };

                var registration = await userManager.CreateAsync(newUser, model.Password);

                if (registration.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, isPersistent: false);
                    TempData["RegisterationMessage"] = "Registration successful!";
                }

                foreach (var error in registration.Errors)
                {
                    errorMessage += error.Description;
                }

                errorMessage = errorMessage.Replace("&#x27;", "");

                TempData["RegisterationErrorMessage"] = errorMessage;
                return RedirectToAction("SignUp", "User");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            // Remove the cart items associated with this user
            await _secureNetworkDbContext.tbl_SNUserCartEntities.ExecuteDeleteAsync();
            await _secureNetworkDbContext.SaveChangesAsync();

            int cartItemCount = await GetCartItemCount();
            TempData["CartItemCount"] = cartItemCount;

            return RedirectToAction("Index", "Home");
        }

        private async Task<int> GetCartItemCount()
        {
            if (signInManager.IsSignedIn(User))
            {
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    return await _secureNetworkDbContext.tbl_SNUserCartEntities
                        .Where(u => u.SNUserId == user.Id)
                        .SumAsync(u => u.SNProductQuantity);
                }
            }

            return 0;
        }
    }
}
