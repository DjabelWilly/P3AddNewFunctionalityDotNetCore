using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILanguageService _languageService;

        public ProductController(IProductService productService, ILanguageService languageService)
        {
            _productService = productService;
            _languageService = languageService;
        }

        /// <summary>
        /// Displays a list of all products.
        /// </summary>
        /// <returns>The Index view with the list of products.</returns>
        public IActionResult Index()
        {
            IEnumerable<ProductViewModel> products = _productService.GetAllProductsViewModel();
            return View(products);
        }

        /// <summary>
        /// Displays a list of all products to authorized users in descending order by ID.
        /// Used for administrative purposes.
        /// </summary>
        /// <returns>The Admin view with the ordered list of products.</returns>
        [Authorize]
        public IActionResult Admin()
        {
            return View(_productService.GetAllProductsViewModel().OrderByDescending(p => p.Id));
        }


        /// <summary>
        /// Displays the form for creating a new product.
        /// Only accessible to authorized users.
        /// </summary>
        /// <returns>The Create view.</returns>
        [Authorize]
        public ViewResult Create()
        {
            return View();
        }


        /// <summary>
        /// Check if the ModelState is valid.
        /// If so, it posts the new product created.
        /// Only accessible to authorized users.
        /// </summary>
        /// <param name="product">the product to add.</param>
        /// <returns>The Admin page view</returns> 
        [Authorize]
        [HttpPost]
        public IActionResult Create(ProductViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Le modèle est valide, on peut enregistrer
            _productService.SaveProduct(product);

            // Redirection vers la page Admin après succès
            return RedirectToAction("Admin");
        }

        /// <summary>
        /// Handles a POST request to delete a product by its ID.
        /// Attempts to delete the product using the product service. 
        /// If an error occurs a message is stored in TempData for display in the view.
        /// Redirects to the Admin action after processing.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <returns>
        /// A redirection to the Admin view, regardless of whether the deletion was successful.
        /// In case of an error, an error message is stored in TempData under "DeleteError".
        /// </returns>
        [Authorize]
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                _productService.DeleteProduct(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["DeleteError"] = ex.Message;
            }
            return RedirectToAction("Admin");
        }
    }
}