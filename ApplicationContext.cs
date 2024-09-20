using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Models.Order> Order { get; set; }
        public DbSet<Models.OrderItem> OrderItem { get; set; }
        public DbSet<Models.Provider> Provider { get; set; }

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=OrdersTable;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}
