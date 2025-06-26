using System.Linq;
using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Services;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    /// <summary>
    /// Controller responsible for managing the shopping cart operations.
    /// </summary>
    public class CartController : Controller
    {
        private readonly ICart _cart;
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="cart">The cart service to manage cart data.</param>
        /// <param name="productService">The service to retrieve product information.</param>
        public CartController(ICart cart, IProductService productService)
        {
            _cart = cart;
            _productService = productService;
        }

        /// <summary>
        /// Displays the contents of the shopping cart.
        /// </summary>
        /// <returns>A view that shows the current cart state.</returns>
        public ViewResult Index()
        {
            Cart cart = _cart as Cart;
            return View(cart);
        }

        /// <summary>
        /// Adds a product to the shopping cart.
        /// </summary>
        /// <param name="id">The ID of the product to add.</param>
        /// <returns>Redirects to the cart index view if successful; otherwise redirects to the product list.</returns>
        [HttpPost]
        public RedirectToActionResult AddToCart(int id)
        {
            Product product = _productService.GetProductById(id);

            if (product != null)
            {
                _cart.AddItem(product, 1);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Product");
            }
        }

        /// <summary>
        /// Removes a product from the shopping cart.
        /// </summary>
        /// <param name="id">The ID of the product to remove.</param>
        /// <returns>Redirects to the cart index view.</returns>
        public RedirectToActionResult RemoveFromCart(int id)
        {
            Product product = _productService.GetAllProducts()
                .FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                _cart.RemoveLine(product);
            }

            return RedirectToAction("Index");
        }
    }
}
