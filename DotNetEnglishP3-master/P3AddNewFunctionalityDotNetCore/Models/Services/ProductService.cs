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
    /// Service responsible for managing products, including retrieving,
    /// validating, updating stock quantities, saving, and deleting products.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ICart _cart;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IStringLocalizer<ProductService> _localizer;

        public ProductService(ICart cart, IProductRepository productRepository,
            IOrderRepository orderRepository, IStringLocalizer<ProductService> localizer)
        {
            _cart = cart;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
        }

        /// <summary>
        /// Retrieves all products and maps them to view models for display purposes.
        /// </summary>
        /// <returns>A list of ProductViewModel instances.</returns>
        public List<ProductViewModel> GetAllProductsViewModel()
        {

            IEnumerable<Product> productEntities = GetAllProducts();
            return MapToViewModel(productEntities);
        }

        /// <summary>
        /// Maps a collection of Product entities to a list of ProductViewModel objects.
        /// </summary>
        /// <param name="productEntities">The collection of Product entities.</param>
        /// <returns>A list of ProductViewModel objects.</returns>
        private static List<ProductViewModel> MapToViewModel(IEnumerable<Product> productEntities)
        {
            List <ProductViewModel> products = new List<ProductViewModel>();
            foreach (Product product in productEntities)
            {
                products.Add(new ProductViewModel
                {
                    Id = product.Id,
                    Stock = product.Quantity,
                    Price = product.Price,
                    Name = product.Name,
                    Description = product.Description,
                    Details = product.Details
                });
            }

            return products;
        }

        /// <summary>
        /// Retrieves all products from the repository.
        /// </summary>
        /// <returns>A list of Product entities.</returns>
        public List<Product> GetAllProducts()
        {
            IEnumerable<Product> productEntities = _productRepository.GetAllProducts();
            return productEntities?.ToList();
        }

        /// <summary>
        /// Retrieves a single product by its ID and maps it to a ProductViewModel.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The corresponding ProductViewModel.</returns>
        public ProductViewModel GetProductByIdViewModel(int id)
        {
            List<ProductViewModel> products = GetAllProductsViewModel().ToList();
            return products.Find(p => p.Id == id);
        }

        /// <summary>
        /// Retrieves a single product entity by its ID.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The corresponding Product entity.</returns>
         public Product GetProductById(int id)
        {
            List<Product> products = GetAllProducts().ToList();
            return products.Find(p => p.Id == id);
        }

        /// <summary>
        /// Asynchronously retrieves a single product by ID from the repository.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The corresponding Product entity.</returns>
        public async Task<Product> GetProduct(int id)
        {
            var product = await _productRepository.GetProduct(id);
            return product;
        }

        /// <summary>
        /// Asynchronously retrieves all products from the repository.
        /// </summary>
        /// <returns>A list of Product entities.</returns>
        public async Task<IList<Product>> GetProduct()
        {
            var products = await _productRepository.GetProduct();
            return products;
        }

        /// <summary>
        /// Updates the stock quantities of all products based on the contents of the shopping cart.
        /// </summary>
        public void UpdateProductQuantities()
        {
            Cart cart = (Cart) _cart;
            foreach (CartLine line in cart.Lines)
            {
                _productRepository.UpdateProductStocks(line.Product.Id, line.Quantity);
            }
        }

        /// <summary>
        /// Saves a new product to the repository after mapping it from a view model.
        /// </summary>
        /// <param name="product">The product view model to save.</param>
        public void SaveProduct(ProductViewModel product)
        {
            var productToAdd = MapToProductEntity(product);
            _productRepository.SaveProduct(productToAdd);
        }

        /// <summary>
        /// Maps a ProductViewModel to a Product entity.
        /// </summary>
        /// <param name="product">The view model to convert.</param>
        /// <returns>The corresponding Product entity.</returns>
        private static Product MapToProductEntity(ProductViewModel product)
        {
            Product productEntity = new Product
            {
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Stock,
                Description = product.Description,
                Details = product.Details
            };
            return productEntity;
        }

        /// <summary>
        /// Deletes a product by its ID and removes it from the cart if present.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public void DeleteProduct(int id)
        {
            // TODO what happens if a product has been added to a cart and has been later removed from the inventory ?
            // delete the product form the cart by using the specific method
            // => the choice is up to the student
            _cart.RemoveLine(GetProductById(id));

            _productRepository.DeleteProduct(id);
        }
    }
}
