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
        [HttpGet]
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
        public async Task<IActionResult> Edit(int id, SNProductsEntity snProductsEntity, IFormFile? newImage)
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
                    var existingProduct = await _secureNetworkDbContext.tbl_SNProducts.FindAsync(id);

                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (newImage != null)
                    {
                        // Delete the existing image file from wwwroot/Images directory
                        if (existingProduct.SNProductImageUrl != null)
                        {
                            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", existingProduct.SNProductImageUrl);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }

                        // Generate a unique file name for the new image
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + newImage.FileName;

                        // Save the new image file to wwwroot/Images directory
                        var newImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", uniqueFileName);
                        await using (var stream = new FileStream(newImagePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }

                        // Update the image file name in the database with the new file name
                        existingProduct.SNProductImageUrl = uniqueFileName;
                    }

                    // Update properties of the product as per the table
                    existingProduct.SNProductName = snProductsEntity.SNProductName;
                    existingProduct.SNProductPrice = snProductsEntity.SNProductPrice;
                    existingProduct.SNProductDescription = snProductsEntity.SNProductDescription;
                    existingProduct.SNProductStock = snProductsEntity.SNProductStock;
                    existingProduct.SNProductRating = snProductsEntity.SNProductRating;
                    existingProduct.SNProductCategory = snProductsEntity.SNProductCategory;
                    existingProduct.ModifieDateTime = DateTime.Now;

                    _secureNetworkDbContext.Update(existingProduct);
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


        public IActionResult Delete(int id)
        {
            TempData["AddSuccess"] = null;
            TempData["AddError"] = null;
            TempData["UpdateSuccess"] = null;
            TempData["UpdateError"] = null;

            var product = _secureNetworkDbContext.tbl_SNProducts.Find(id);
            if (product != null)
            {
                // Get the filename of the image associated with the product
                string? filename = product.SNProductImageUrl; // Assuming ImageUrl contains the filename

                // Remove the product from the database
                _secureNetworkDbContext.tbl_SNProducts.Remove(product);
                _secureNetworkDbContext.SaveChanges();

                // Delete the corresponding file from the image folder in the wwwroot directory
                if (filename != null)
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", filename);
                    if (System.IO.File.Exists(imagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception (log, display error message, etc.)
                            TempData["DeleteError"] = "Error deleting image file: " + ex.Message;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["DeleteError"] = "Image file not found.";
                    }
                }
            }
            else
            {
                TempData["DeleteError"] = "Product not found.";
            }

            return RedirectToAction("Index");
        }
    }
}
