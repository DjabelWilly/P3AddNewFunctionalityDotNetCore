using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class ProductServiceIntegrationTests : IClassFixture<IntegrationTestFactory>
    {
        private readonly IntegrationTestFactory _factory;

        public ProductServiceIntegrationTests(IntegrationTestFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void DeleteProduct_ShouldRemoveProduct_WhenNotInCart()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var cart = scope.ServiceProvider.GetRequiredService<ICart>();
            cart.Clear();

            // Act
            productService.DeleteProduct(1); // suppression produitTest1
            var deleted = productService.GetProductById(1);

            // Assert
            Assert.Null(deleted);
        }

        [Fact]
        public void DeleteProduct_ShouldThrow_WhenProductIsInCart()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var cart = scope.ServiceProvider.GetRequiredService<ICart>();
            var product = productService.GetAllProducts().First();

            // Act 
            cart.AddItem(product, 1);

            // Assert
            Assert.Throws<InvalidOperationException>(() => productService.DeleteProduct(product.Id));
        }

        [Fact]
        public void UpdateProductQuantities_ShouldDecreaseStock_WhenCartValidated()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var cart = scope.ServiceProvider.GetRequiredService<ICart>();
            var product = productService.GetProductById(3); // produitTest3
            var initialStock = product.Quantity;
            cart.AddItem(product, 2);

            // Act
            productService.UpdateProductQuantities();

            // Assert
            var updated = productService.GetProductById(product.Id);
            Assert.Equal(initialStock - 2, updated.Quantity);
        }

        [Fact]
        public void GetAllProducts_ShouldReturnSeededProductsEntities()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Act
            var products = productService.GetAllProducts();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Name == "produitTest1");
            Assert.Contains(products, p => p.Name == "produitTest2");
            Assert.Contains(products, p => p.Name == "produitTest3");
        }

        [Fact]
        public void GetProductById_ShouldReturnCorrectEntity()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var allProducts = productService.GetAllProducts();
            var target = allProducts.First();
            var targetId = target.Id;

            // Act
            var product = productService.GetProductById(targetId);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(targetId, product.Id);
            Assert.Equal(target.Name, product.Name);
            Assert.Equal(target.Quantity, product.Quantity);
            Assert.Equal(target.Price, product.Price);
        }

        [Fact]
        public void SaveProduct_ShouldAddProductToDatabase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var newProduct = new ProductViewModel
            {
                Name = "ProduitIntegrationTest",
                Description = "Produit créé via test d'intégration",
                Stock = 15,
                Price = 42.50
            };

            // Act
            productService.SaveProduct(newProduct);
            var dbProduct = productService.GetAllProductsViewModel()
                              .FirstOrDefault(p => p.Name == "ProduitIntegrationTest");

            // Assert
            Assert.NotNull(dbProduct);
            Assert.Equal(15, dbProduct.Stock);
            Assert.Equal(42.50, dbProduct.Price);
        }
    }
}
