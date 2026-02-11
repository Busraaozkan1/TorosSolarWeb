using Microsoft.EntityFrameworkCore;
using TorosSolar.Models; 

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

        // Favorilerin tutulacağı yeni tablo (Eski yapıya dokunmadan eklendi)
        public DbSet<Favorite> Favorites { get; set; }

        // HATA ÇÖZÜMÜ: 'User' ismi çakışmasını önlemek için tam namespace yolu ile tanımlıyoruz.
        // Bu sayede AccountController içindeki "BinaryReader" hatası da zincirleme olarak düzelecektir.
        public DbSet<TorosSolar.Models.User> Users { get; set; } = null!;
    }
}