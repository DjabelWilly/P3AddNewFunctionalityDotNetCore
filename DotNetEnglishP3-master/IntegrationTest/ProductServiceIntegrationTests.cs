using Microsoft.Extensions.DependencyInjection;
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

            // Assert
            var dbProduct = productService.GetAllProductsViewModel()
                              .FirstOrDefault(p => p.Name == "ProduitIntegrationTest");

            Assert.NotNull(dbProduct);
            Assert.Equal(15, dbProduct.Stock);
            Assert.Equal(42.50, dbProduct.Price);
        }

        [Fact]
        public void DeleteProduct_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Créer un produit à supprimer
            var productToDelete = new ProductViewModel
            {
                Name = "ProduitASupprimer",
                Description = "Produit créé pour test de suppression",
                Stock = 10,
                Price = 25.00
            };

            productService.SaveProduct(productToDelete);

            var dbProduct = productService.GetAllProductsViewModel()
                              .FirstOrDefault(p => p.Name == "ProduitASupprimer");

            Assert.NotNull(dbProduct); // Vérifie que le produit a bien été créé
            Assert.Equal(dbProduct.Name, productToDelete.Name);

            // Act
            productService.DeleteProduct(dbProduct.Id);

            // Assert
            var deletedProduct = productService.GetAllProductsViewModel()
                                 .FirstOrDefault(p => p.Name == "ProduitASupprimer");

            Assert.Null(deletedProduct); // Vérifie que le produit a bien été supprimé
        }
    }
}
