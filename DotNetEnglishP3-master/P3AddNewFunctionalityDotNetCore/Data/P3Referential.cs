using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Data;

namespace P3AddNewFunctionalityDotNetCore.Data
{
    /// <summary>
    /// Represents the Entity Framework Core database context for the application.
    /// Manages access to Order, OrderLine, and Product entities.
    /// </summary>
    public class P3Referential : DbContext
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        private IDbConnection DbConnection { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="P3Referential"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <param name="config">The application configuration used to retrieve the connection string.</param>
        public P3Referential(DbContextOptions<P3Referential> options, IConfiguration config)
            : base(options)
        {
            DbConnection = new SqlConnection(config.GetConnectionString("P3Referential"));
        }

        /// <summary>
        /// Gets or sets the Orders in the database.
        /// </summary>
        public virtual DbSet<Order> Order { get; set; }

        /// <summary>
        /// Gets or sets the OrderLines in the database.
        /// </summary>
        public virtual DbSet<OrderLine> OrderLine { get; set; }

        /// <summary>
        /// Gets or sets the Products in the database.
        /// </summary>
        public virtual DbSet<Product> Product { get; set; }

        /// <summary>
        /// Configures the database context with the SQL Server provider and retry logic.
        /// </summary>
        /// <param name="optionsBuilder">The builder used to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    DbConnection.ConnectionString,
                    providerOptions => providerOptions.EnableRetryOnFailure());
            }
        }

        /// <summary>
        /// Configures the entity relationships and database schema mappings.
        /// </summary>
        /// <param name="modelBuilder">The builder used to define the model for the context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<OrderLine>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                      .HasDatabaseName("IX_OrderLineEntity_OrderEntityId");

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.OrderLine)
                      .HasForeignKey(d => d.OrderId)
                      .HasConstraintName("FK_OrderLineEntity_OrderEntity_OrderEntityId")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.OrderLine)
                      .HasForeignKey(d => d.ProductId)
                      .HasConstraintName("FK__OrderLine__Produ__52593CB8")
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
