using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Models.Repositories
{
    /// <summary>
    /// Provides methods for storing and retrieving order data from the database.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly P3Referential _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used to access order data.</param>
        public OrderRepository(P3Referential context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves a new order to the database.
        /// </summary>
        /// <param name="order">The order to save.</param>
        public void Save(Order order)
        {
            _context.Order.Add(order);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retrieves a specific order by its ID, including its order lines and product details.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order with its lines and product data, or null if not found.</returns>
        public async Task<Order> GetOrder(int? id)
        {
            var orderEntity = await _context.Order
                .Include(x => x.OrderLine)
                .ThenInclude(product => product.Product)
                .SingleOrDefaultAsync(m => m.Id == id);
            return orderEntity;
        }

        /// <summary>
        /// Retrieves all orders from the database, including their order lines and product details.
        /// </summary>
        /// <returns>A list of all orders with associated data.</returns>
        public async Task<IList<Order>> GetOrders()
        {
            var orders = await _context.Order
                .Include(x => x.OrderLine)
                .ThenInclude(product => product.Product)
                .ToListAsync();
            return orders;
        }
    }
}
