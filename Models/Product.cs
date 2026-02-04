using System.ComponentModel.DataAnnotations;

namespace TorosSolar.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur")]
        public string Name { get; set; } = string.Empty; // Boş kalmasın diye varsayılan değer ekledik

        [Required(ErrorMessage = "Fiyat zorunludur")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur")]
        public string Description { get; set; } = string.Empty; // Buraya da varsayılan değer ekledik

        public string? ImageUrl { get; set; } // Soru işareti (?), bu alanın boş (null) olabileceğini söyler

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}