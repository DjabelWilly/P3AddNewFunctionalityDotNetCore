using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models.Entities;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public static class TestDataSeeder
    {
        public static void SeedTestProducts(P3Referential context)
        {
            // Ajoute les produits de test
            context.Product.AddRange(
                new Product
                {
                    Name = "produitTest1",
                    Description = "desc du produit1",
                    Quantity = 7,
                    Price = 92.50
                },
                new Product
                {
                    Name = "produitTest2",
                    Description = "desc du produit2",
                    Quantity = 10,
                    Price = 9.99
                },
                new Product
                {
                    Name = "produitTest3",
                    Description = "desc du produit3",
                    Quantity = 5,
                    Price = 69.99
                }
            );

            // Sauvegarde les produits
            context.SaveChanges();
        }
    }
}
