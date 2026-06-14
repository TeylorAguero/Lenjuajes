using Microsoft.EntityFrameworkCore;

namespace ApiWebSecurityCloud.Models
{
    public class DbContextSecurity : DbContext
    {
        public DbContextSecurity(DbContextOptions<DbContextSecurity> options) : base(options)
        {

        }

        // Propiedades para manejar las tablas
        public DbSet<Usuario> Usuarios { get; set; }

    }
}

