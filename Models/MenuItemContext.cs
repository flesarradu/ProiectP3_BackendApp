using Microsoft.EntityFrameworkCore;

namespace ProiectP3_BackendApp.Models
{
    public class MenuItemContext : DbContext
    {
        public MenuItemContext(DbContextOptions<MenuItemContext> options)
           : base(options)
        {
        }
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
    }
}
