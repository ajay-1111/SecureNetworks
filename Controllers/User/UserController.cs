// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Models.DBModels;
using SecureNetworks.Models.ViewModel;

namespace SecureNetworks.Controllers.User
{
    public class UserController : Controller
    {
        private readonly SecureNetworkDBContext _secureNetworkDbContext;

        private readonly SignInManager<ApplicationUserDBEntity> signInManager;
        
        private readonly UserManager<ApplicationUserDBEntity> userManager;

        public UserController(SecureNetworkDBContext _secureNetworkDbContext, SignInManager<ApplicationUserDBEntity> signInManager, UserManager<ApplicationUserDBEntity> userManager)
        {
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
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Products");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
                };

                var registration = await userManager.CreateAsync(newUser, model.Password);

                if (registration.Succeeded)
                {
                    await signInManager.SignInAsync(newUser, isPersistent: false);
                    TempData["RegisterationMessage"] = "Registration successful!";
                    return RedirectToAction("SignIn", "User");
                }

                foreach (var error in registration.Errors)
                {
                    errorMessage += error.Description;
                }

                TempData["RegisterationErrorMessage"] = errorMessage;
                return RedirectToAction("SignUp", "User");
            }

            return View(model);
        }
    }
}
