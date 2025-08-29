using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;
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

        // Récupère les 3 produits crées par TestDataSeeder 
        [Fact]
        public void GetAllProducts_ShouldReturnSeededProducts()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<P3Referential>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Act
            var products = productService.GetAllProductsViewModel();

            // Assert
            Assert.Contains(products, p => p.Name == "produitTest1");
            Assert.Contains(products, p => p.Name == "produitTest2");
            Assert.Contains(products, p => p.Name == "produitTest3");
        }

        // Retourne le produit selon l'id
        [Fact]
        public void GetProductById_ShouldReturnCorrectProduct()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<P3Referential>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Act
            var allProducts = productService.GetAllProductsViewModel();
            var target = allProducts.First();

            var result = productService.GetProductByIdViewModel(target.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(target.Name, result.Name);
        }

        // Ajoute un produit dans la DB
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

        // Supprime le produitTest1 de la DB
        [Fact]
        public void DeleteProduct_ShouldRemoveProduct_WhenNotInCart()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<P3Referential>();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var cart = scope.ServiceProvider.GetRequiredService<ICart>();

            // Vide le panier pour être sûr qu'aucun produit n'y est
            cart.Clear();

            var product = productService.GetAllProductsViewModel().First(); // produitTest1

            // Act
            productService.DeleteProduct(product.Id);
            var deleted = productService.GetProductById(product.Id);

            // Assert
            Assert.Null(deleted);
        }

        // Vérifie que le produit n'est pas présent Cart avant de le supprimer
        // Lève une Exception si c'est le cas.
        [Fact]
        public void DeleteProduct_ShouldThrow_WhenProductIsInCart()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var cart = scope.ServiceProvider.GetRequiredService<ICart>();

            // Act 
            var product = productService.GetAllProducts().First();
            cart.AddItem(product, 1); // produit ajouté au panier

            // Assert
            Assert.Throws<InvalidOperationException>(
                () => productService.DeleteProduct(product.Id)
                );
        }

        // Mise à jour de la quantité d'un produit.
        [Fact]
        public void UpdateProductQuantities_ShouldDecreaseStock()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<P3Referential>();

            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var cart = scope.ServiceProvider.GetRequiredService<ICart>();

            // Act
            var product = productService.GetProductById(3); // produitTest3 -> Quantity = 5
            var initialStock = product.Quantity;

            cart.AddItem(product, 2); // ajoute 2 au panier

            productService.UpdateProductQuantities();

            // Assert
            var updatedProduct = productService.GetProductById(product.Id);
            Assert.Equal(initialStock - 2, updatedProduct.Quantity); // 5 - 2 = 3
        }

        // Vérifie que la méthode retourne bien les entités depuis la base, et non pas les ViewModels.
        [Fact]
        public void GetAllProducts_ShouldReturnSeededProductsEntities()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            // Act
            var products = productService.GetAllProducts(); // méthode directe sur Product

            // Assert
            Assert.NotNull(products);
            Assert.Equal(3, products.Count); // il y a 3 produits seedés
            Assert.Contains(products, p => p.Name == "produitTest1");
            Assert.Contains(products, p => p.Name == "produitTest2");
            Assert.Contains(products, p => p.Name == "produitTest3");
        }

        // Vérifie la récupération d’une entité précise par Id, directement depuis la base.
        [Fact]
        public void GetProductById_ShouldReturnCorrectEntity()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            var allProducts = productService.GetAllProducts();
            var target = allProducts.First(); // récupère un produit existant
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
    }
}
