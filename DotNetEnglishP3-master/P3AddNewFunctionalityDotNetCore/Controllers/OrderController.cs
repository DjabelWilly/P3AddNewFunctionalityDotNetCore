using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    /// <summary>
    /// Controller responsible for handling customer orders.
    /// </summary>
    public class OrderController : Controller
    {
        private readonly ICart _cart;
        private readonly IOrderService _orderService;
        private readonly IStringLocalizer<OrderController> _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="cart">The cart service instance.</param>
        /// <param name="service">The order service for processing orders.</param>
        /// <param name="localizer">The localizer for multilingual support.</param>
        public OrderController(ICart cart, IOrderService service, IStringLocalizer<OrderController> localizer)
        {
            _cart = cart;
            _orderService = service;
            _localizer = localizer;
        }

        /// <summary>
        /// Displays the order form to the user.
        /// </summary>
        /// <returns>The view containing the order form.</returns>
        public ViewResult Index()
        {
            return View(new OrderViewModel());
        }

        /// <summary>
        /// Handles submission of the order form.
        /// Validates the cart and saves the order if valid.
        /// </summary>
        /// <param name="order">The order view model submitted by the user.</param>
        /// <returns>Redirects to the Completed view if successful; otherwise, redisplays the form with validation errors.</returns>
        [HttpPost]
        public IActionResult Index(OrderViewModel order)
        {
            if (!((Cart)_cart).Lines.Any())
            {
                ModelState.AddModelError("", _localizer["CartEmpty"]);
            }

            if (ModelState.IsValid)
            {
                order.Lines = ((Cart)_cart)?.Lines.ToArray();
                _orderService.SaveOrder(order);
                return RedirectToAction(nameof(Completed));
            }
            else
            {
                return View(order);
            }
        }

        /// <summary>
        /// Displays the order completion page and clears the cart.
        /// </summary>
        /// <returns>The view confirming the order has been completed.</returns>
        public ViewResult Completed()
        {
            _cart.Clear();
            return View();
        }
    }
}
