using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Models.Entities;

namespace P3AddNewFunctionalityDotNetCore.Data
{
    /// <summary>
    /// Provides initial seed data for the application's database.
    /// </summary>
    public class SeedData
    {
        /// <summary>
        /// Initializes the database with default product entries if none exist.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve the DbContext options.</param>
        /// <param name="config">The application configuration used by the DbContext.</param>
        public static void Initialize(IServiceProvider serviceProvider, IConfiguration config)
        {
            using var context = new P3Referential(
                serviceProvider.GetRequiredService<DbContextOptions<P3Referential>>(), config);

            // Exit if the database already contains product data
            if (context.Product.Any())
            {
                return;
            }

            // Add initial products
            context.Product.AddRange(
                new Product
                {
                    Name = "Echo Dot",
                    Description = "(2nd Generation) - Black",
                    Quantity = 10,
                    Price = 92.50
                },

                new Product
                {
                    Name = "Anker 3ft / 0.9m Nylon Braided",
                    Description = "Tangle-Free Micro USB Cable",
                    Quantity = 20,
                    Price = 9.99
                },

                new Product
                {
                    Name = "JVC HAFX8R Headphone",
                    Description = "Riptidz, In-Ear",
                    Quantity = 30,
                    Price = 69.99
                },

                new Product
                {
                    Name = "VTech CS6114 DECT 6.0",
                    Description = "Cordless Phone",
                    Quantity = 40,
                    Price = 32.50
                },

                new Product
                {
                    Name = "NOKIA OEM BL-5J",
                    Description = "Cell Phone",
                    Quantity = 50,
                    Price = 895.00
                }
            );

            // Save changes to the database
            context.SaveChanges();
        }
    }
}
