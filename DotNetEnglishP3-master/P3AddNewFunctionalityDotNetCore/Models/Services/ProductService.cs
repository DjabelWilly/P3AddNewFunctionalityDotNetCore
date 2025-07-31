using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Models.Services
{
    /// <summary>
    /// Service responsible for managing products, including retrieval, creation, update, deletion, and cart-related logic.
    /// </summary>
    public class ProductService : IProductService
    {
        /// <summary>
        /// Represents the shopping cart instance used for managing cart operations.
        /// </summary>
        private readonly ICart _cart;

        /// <summary>
        /// Provides access to product-related data storage operations.
        /// </summary>
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Provides access to order-related data storage operations.
        /// </summary>
        private readonly IOrderRepository _orderRepository;

        /// <summary>
        /// Provides localized strings for the ProductService class.
        /// </summary>
        private readonly IStringLocalizer<ProductService> _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class with the specified dependencies.
        /// </summary>
        /// <param name="cart">The shopping cart implementation.</param>
        /// <param name="productRepository">The product repository implementation.</param>
        /// <param name="orderRepository">The order repository implementation.</param>
        /// <param name="localizer">The string localizer for localization support.</param>
        public ProductService(ICart cart, IProductRepository productRepository,
            IOrderRepository orderRepository, IStringLocalizer<ProductService> localizer)
        {
            _cart = cart;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
        }

        /// <summary>
        /// Retrieves all products from the repository and maps them to their corresponding view models.
        /// </summary>
        /// <returns>A list of <see cref="ProductViewModel"/> representing all products.</returns>
        public List<ProductViewModel> GetAllProductsViewModel()
        {
            IEnumerable<Product> productEntities = GetAllProducts();
            return MapToViewModel(productEntities);
        }

        /// <summary>
        /// Maps a collection of <see cref="Product"/> entities to a list of <see cref="ProductViewModel"/> objects.
        /// </summary>
        /// <param name="productEntities">The collection of product entities to map.</param>
        /// <returns>A list of mapped view models.</returns>
        private static List<ProductViewModel> MapToViewModel(IEnumerable<Product> productEntities)
        {
            var culture = CultureInfo.CurrentCulture;

            return productEntities.Select(product => new ProductViewModel
            {
                Id = product.Id,
                Stock = product.Quantity,
                Price = (double)product.Price,
                Name = product.Name,
                Description = product.Description,
                Details = product.Details
            }).ToList();
        }

        /// <summary>
        /// Retrieves all product entities from the repository.
        /// </summary>
        /// <returns>A list of <see cref="Product"/> objects.</returns>
        public List<Product> GetAllProducts()
        {
            IEnumerable<Product> productEntities = _productRepository.GetAllProducts();
            return productEntities?.ToList();
        }

        /// <summary>
        /// Retrieves a single product view model by its identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The <see cref="ProductViewModel"/> with the given ID, or null if not found.</returns>
        public ProductViewModel GetProductByIdViewModel(int id)
        {
            return GetAllProductsViewModel().Find(p => p.Id == id);
        }

        /// <summary>
        /// Retrieves a single product entity by its identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The <see cref="Product"/> with the given ID, or null if not found.</returns>
        public Product GetProductById(int id)
        {
            return GetAllProducts().Find(p => p.Id == id);
        }

        /// <summary>
        /// Asynchronously retrieves a single product entity by its identifier from the repository.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the product entity.</returns>
        public async Task<Product> GetProduct(int id)
        {
            return await _productRepository.GetProduct(id);
        }

        /// <summary>
        /// Asynchronously retrieves all product entities from the repository.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The result contains a list of product entities.</returns>
        public async Task<IList<Product>> GetProduct()
        {
            return await _productRepository.GetProduct();
        }

        /// <summary>
        /// Updates the stock quantities of products in the cart by decreasing their quantity in the repository.
        /// </summary>
        public void UpdateProductQuantities()
        {
            Cart cart = (Cart)_cart;
            foreach (CartLine line in cart.Lines)
            {
                _productRepository.UpdateProductStocks(line.Product.Id, line.Quantity);
            }
        }

        /// <summary>
        /// Saves a new product to the repository based on the provided view model.
        /// </summary>
        /// <param name="product">The product view model to be saved.</param>
        public void SaveProduct(ProductViewModel product)
        {
            var productToAdd = MapToProductEntity(product);
            _productRepository.SaveProduct(productToAdd);
        }

        /// <summary>
        /// Maps a <see cref="ProductViewModel"/> to a <see cref="Product"/> entity.
        /// </summary>
        /// <param name="product">The view model to map.</param>
        /// <returns>The mapped product entity.</returns>
        private static Product MapToProductEntity(ProductViewModel product)
        {
            return new Product
            {
                Name = product.Name,
                Price = (double)product.Price,
                Quantity = product.Stock,
                Description = product.Description,
                Details = product.Details
            };
        }

        /// <summary>
        /// Deletes a product by its ID, removing it from the repository
        /// only IF the product is not present in cart instance. 
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public void DeleteProduct(int id)
        {
            // Check if the product to delete is not present in cart.
            // If it is present, cannot delete product and sends a message to UI.
            // If it is not, delete the product from the cart and repository.
            var product = GetProductById(id);

            Cart cart = _cart as Cart;

            if (cart != null)
            {
                foreach (CartLine line in cart.Lines)
                {
                    if (line.Product.Id == id)
                    {
                        // send message to front 
                        throw new InvalidOperationException(_localizer["CannotDeleteProduct"]);
                    }
                }
            }
            _productRepository.DeleteProduct(id);
        }
    }
}
