using System.ComponentModel.DataAnnotations;

namespace TorosSolar.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gerekli")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Güvenlik sorusunu yanıtlayın")]
        public int SecurityAnswer { get; set; } // Kullanıcının girdiği cevap

        public int Number1 { get; set; } // Sistemdeki ilk sayı
        public int Number2 { get; set; } // Sistemdeki ikinci sayı
    }
}