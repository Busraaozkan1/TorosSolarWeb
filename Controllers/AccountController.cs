using Microsoft.AspNetCore.Mvc;
using TorosSolar.Models;

namespace TorosSolar.Controllers
{
    public class AccountController : Controller
    {
        // 1. GİRİŞ SAYFASI (GET)
        [HttpGet]
        public IActionResult Login()
        {
            Random rnd = new Random();
            var model = new LoginViewModel
            {
                Number1 = rnd.Next(1, 10),
                Number2 = rnd.Next(1, 10)
            };
            return View(model);
        }

        // 2. GİRİŞ İŞLEMİ (POST)
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            // Güvenlik sorusu kontrolü
            if (model.SecurityAnswer != (model.Number1 + model.Number2))
            {
                ModelState.AddModelError("", "Güvenlik sorusu hatalı!");
            }
            
            // Kullanıcı adı ve şifre kontrolü
            if (model.Username == "admin" && model.Password == "1234")
            {
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Product");
                }
            }
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
            }

            // Hata varsa yeni sayılarla geri dön
            Random rnd = new Random();
            model.Number1 = rnd.Next(1, 10);
            model.Number2 = rnd.Next(1, 10);
            return View(model);
        }

        // 3. ŞİFRE DEĞİŞTİRME SAYFASI (GET)
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // 4. ŞİFRE DEĞİŞTİRME İŞLEMİ (POST)
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            if (currentPassword == "1234") 
            {
                TempData["Success"] = "Şifreniz başarıyla güncellendi!";
                return RedirectToAction("Index", "Product");
            }
            
            ViewBag.Error = "Mevcut şifre hatalı!";
            return View();
        }

        // 5. ÇIKIŞ YAP
        public IActionResult Logout()
        {
            // Şimdilik basitçe ana sayfaya yönlendiriyoruz
            return RedirectToAction("Index", "Home");
        }
    }
}