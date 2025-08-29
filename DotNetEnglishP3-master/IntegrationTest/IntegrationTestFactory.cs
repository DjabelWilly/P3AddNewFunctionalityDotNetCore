using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class IntegrationTestFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Surcharge pour utiliser le fichier config de test
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Test.json", optional: false);
            });

            builder.ConfigureServices(services =>
            {
                // Construit un scope pour accéder au DbContext
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<P3Referential>();

                // Réinitialisation complète de la base
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated(); 

                // Seed les données de test
                TestDataSeeder.SeedTestProducts(db);
            });
        }
    }
}
