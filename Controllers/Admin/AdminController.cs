// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdminController.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNetworks.DataBaseContext;
using SecureNetworks.Helpers;
using SecureNetworks.Models.DBModels;

namespace SecureNetworks.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly SecureNetworkDBContext _secureNetworkDbContext;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(SecureNetworkDBContext secureNetworkDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _secureNetworkDbContext = secureNetworkDbContext;
        }

        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var products = _secureNetworkDbContext.tbl_SNProducts.OrderBy(p => p.SNProductId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalProducts = _secureNetworkDbContext.tbl_SNProducts.Count();

            var model = new PaginationHelper<SNProductsEntity>(products, totalProducts, page, pageSize);

            return View(model);
        }

        // Action method to display form for adding a new product
        [HttpPost]
        public IActionResult Create()
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SNProductsEntity snProductsEntity, IFormFile? productImage)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a file is uploaded
                    if (productImage is { Length: > 0 })
                    {
                        // Generate a unique filename for the image
                        var uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(productImage.FileName);

                        // Get the path of the wwwroot/img folder where images will be stored
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                        // Combine the unique filename with the path to store the image
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Copy the uploaded file to the specified path
                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await productImage.CopyToAsync(stream);
                        }

                        // Update the ImageUrl property of the product with the new filename
                        snProductsEntity.SNProductImageUrl = uniqueFileName;
                    }

                    // Set CreateDateTime and ModifieDateTime
                    snProductsEntity.CreateDateTime = DateTime.Now;
                    snProductsEntity.ModifieDateTime = DateTime.Now;

                    // Add the product to the database
                    _secureNetworkDbContext.tbl_SNProducts.Add(snProductsEntity);
                    await _secureNetworkDbContext.SaveChangesAsync();

                    TempData["AddSuccess"] = $"ProductID {snProductsEntity.SNProductId} has been added successfully.";

                    // Redirect to the product list page after successful creation
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log or handle the exception appropriately
                    TempData["AddError"] = $"Exception while adding the new product : {ex.Message}";
                }
            }

            // If the model state is not valid, return the view with the model data and errors
            return View(snProductsEntity);
        }

        // Action method to display form for updating product details
        [HttpPost]
        public IActionResult Edit(int id)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;
            var product = _secureNetworkDbContext.tbl_SNProducts.Find(id);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SNProductsEntity snProductsEntity)
        {
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;

            if (id != snProductsEntity.SNProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update other properties of the product as usual
                    _secureNetworkDbContext.Update(snProductsEntity);
                    await _secureNetworkDbContext.SaveChangesAsync();
                    TempData["UpdateSuccess"] = $"ProductID {snProductsEntity.SNProductId} has been updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["UpdateError"] = $"Exception updating the product : {ex.Message}.";
                }
            }
            return RedirectToAction("Index");
        }

        // Action method to delete a product
        public IActionResult Delete(int id)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;

            var product = _secureNetworkDbContext.tbl_SNProducts.Find(id);
            if (product != null) _secureNetworkDbContext.tbl_SNProducts.Remove(product);
            _secureNetworkDbContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
