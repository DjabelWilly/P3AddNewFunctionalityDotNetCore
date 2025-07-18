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