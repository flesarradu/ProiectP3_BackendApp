using Microsoft.EntityFrameworkCore;
namespace ProiectP3_BackendApp.Models
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options)
           : base(options)
        {
        }
        public DbSet<Order> Orders { get; set; } = null!;
    }
}
