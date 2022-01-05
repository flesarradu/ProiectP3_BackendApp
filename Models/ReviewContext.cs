using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ProiectP3_BackendApp.Models
{
    public class ReviewContext : DbContext
    {
        public ReviewContext(DbContextOptions<ReviewContext> options)
            : base(options)
        {
        }
        public DbSet<Review> Reviews { get; set; } = null!;
    }
}
