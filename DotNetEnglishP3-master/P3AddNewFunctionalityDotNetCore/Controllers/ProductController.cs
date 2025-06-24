using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
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
        /// Handles the POST request to create a new product. 
        /// Validates the product using the product service, adds any validation errors to the ModelState, 
        /// and saves the product if the model is valid. 
        /// Redirects to the Admin page upon successful creation, or returns the form view with validation errors otherwise.
        /// </summary>
        /// <param name="product">The product data submitted from the form.</param>
        /// <returns>A redirect to the Admin action if successful; otherwise, the Create view with validation messages.</returns>
        [Authorize]
        [HttpPost]
        public IActionResult Create(ProductViewModel product)
        {
            List<string> modelErrors = _productService.CheckProductModelErrors(product);           

            foreach (string error in modelErrors)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                _productService.SaveProduct(product);
                return RedirectToAction("Admin");
            }
            else
            {
                return View(product);
            }
        }

        /// <summary>
        /// Handles the POST request to delete a product by its ID.
        /// Calls the product service to remove the product and then redirects to the Admin view.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>A redirect to the Admin action after deletion.</returns>
        [Authorize]
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction("Admin");
        }
    }
}