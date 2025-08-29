using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class DBConnectionTests : IClassFixture<IntegrationTestFactory>
    {
        private readonly IntegrationTestFactory _factory;

        public DBConnectionTests(IntegrationTestFactory factory)
        {
            _factory = factory;
        }

        // Test de connexion à la DB
        // Vérifie bien qu’on utilise la DB de test.
        [Fact]
        public void Database_ShouldUse_TestConnection()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<P3Referential>();

            var conn = db.Database.GetDbConnection().ConnectionString;

            Assert.Contains("P3Referential_Test", conn); 
        }
    }

}
