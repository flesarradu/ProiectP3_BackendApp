using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ProiectP3_BackendApp.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) 
            : base(options) { 
        }
        public DbSet<User> Users { get; set; } = null!;
    }
}
