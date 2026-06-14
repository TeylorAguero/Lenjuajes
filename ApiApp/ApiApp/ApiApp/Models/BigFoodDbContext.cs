using Microsoft.EntityFrameworkCore;

namespace ApiApp.Models
{
    public class BigFoodDbContext : DbContext
    {
        public BigFoodDbContext(DbContextOptions<BigFoodDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}