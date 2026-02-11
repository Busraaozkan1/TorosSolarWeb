using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ForeignKey için gerekli

namespace TorosSolar.Models
{
    public class Favorite
    {
        [Key] // Bu birincil anahtar
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        // Ürün ile ilişki kuruyoruz
        [Required]
        public int ProductId { get; set; }

        // --- İLİŞKİ TANIMI ---
        // Bu satır veritabanında ProductId kolonunun Product tablosuna bağlı olduğunu söyler
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}