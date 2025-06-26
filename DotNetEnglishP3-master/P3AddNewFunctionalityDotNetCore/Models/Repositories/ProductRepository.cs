using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Models.Repositories
{
    /// <summary>
    /// Provides methods to manage and access product data from the database.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly P3Referential _context;
        //private static P3Referential _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ProductRepository(P3Referential context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>The product if found; otherwise, null.</returns>
        public async Task<Product> GetProduct(int id)
        {
            return await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Retrieves all products asynchronously.
        /// </summary>
        /// <returns>A list of products.</returns>
        public async Task<IList<Product>> GetProduct()
        {
            return await _context.Product.ToListAsync();
        }

        /// <summary>
        /// Retrieves all products in the inventory (synchronously).
        /// </summary>
        /// <returns>An enumerable of all products.</returns>
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Product.Where(p => p.Id > 0).ToList();
        }

        /// <summary>
        /// Updates the stock quantity of a product and removes it if quantity becomes 0.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="quantityToRemove">The quantity to subtract from current stock.</param>
        public void UpdateProductStocks(int id, int quantityToRemove)
        {
            Product product = _context.Product.First(p => p.Id == id);
            product.Quantity -= quantityToRemove;

            if (product.Quantity == 0)
            {
                _context.Product.Remove(product);
            }
            else
            {
                _context.Product.Update(product);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Saves a new product to the database.
        /// </summary>
        /// <param name="product">The product to save.</param>
        public void SaveProduct(Product product)
        {
            if (product != null)
            {
                _context.Product.Add(product);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        public void DeleteProduct(int id)
        {
            Product product = _context.Product.First(p => p.Id == id);
            if (product != null)
            {
                _context.Product.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}
