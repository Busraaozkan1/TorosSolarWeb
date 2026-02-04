using Microsoft.EntityFrameworkCore;
using TorosSolar.Models; // Models klasörünün adından emin ol


namespace TorosSolar.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Adminin ekleyeceği ürünler bu tabloya kaydedilecek
        public DbSet<Product> Products { get; set; }
    }
}