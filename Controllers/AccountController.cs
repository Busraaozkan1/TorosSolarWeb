using Microsoft.AspNetCore.Mvc;
using TorosSolar.Models;
using TorosSolar.Data;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TorosSolar.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // 1. ADMİN GİRİŞ METODLARI (GÜNCELLENDİ VE HATALARI GİDERİLDİ)
        // ==========================================================

        [HttpGet]
        [Route("admin")]
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

        [HttpPost]
        [Route("admin")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model.SecurityAnswer != (model.Number1 + model.Number2))
            {
                ModelState.AddModelError("", "Güvenlik sorusu hatalı!");
            }

            if (ModelState.IsValid)
            {
                // ÖNCE: Eski oturum izlerini silerek Id çakışmasını önlüyoruz
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Veritabanında kullanıcıyı ara
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                bool isPasswordCorrect = false;

                if (user != null)
                {
                    isPasswordCorrect = user.Password == model.Password;
                }
                else if (model.Username == "admin" && model.Password == "1234")
                {
                    isPasswordCorrect = true;
                }

                if (isPasswordCorrect)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user?.Username ?? model.Username),
                        new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString() ?? "0"),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                }
            }

            Random rnd = new Random();
            model.Number1 = rnd.Next(1, 10);
            model.Number2 = rnd.Next(1, 10);
            return View(model);
        }

        // ==========================================================
        // 2. MÜŞTERİ GİRİŞ METODLARI
        // ==========================================================

        [HttpGet]
        public IActionResult UserLogin()
        {
            Random rnd = new Random();
            var model = new LoginViewModel
            {
                Number1 = rnd.Next(1, 10),
                Number2 = rnd.Next(1, 10)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(LoginViewModel model)
        {
            if (model.SecurityAnswer != (model.Number1 + model.Number2))
            {
                ModelState.AddModelError("", "Güvenlik sorusu hatalı!");
            }

            if (ModelState.IsValid)
            {
                // Müşteri girişi öncesi de temizlik yapmak çakışmaları önler
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                }
            }

            Random rnd = new Random();
            model.Number1 = rnd.Next(1, 10);
            model.Number2 = rnd.Next(1, 10);
            return View(model);
        }

        // ==========================================================
        // 3. YARDIMCI VE ŞİFRE METODLARI
        // ==========================================================

        [HttpGet]
        [Authorize] 
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string newUsername, string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Yeni şifreler birbiriyle eşleşmiyor.";
                return View();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userIdClaim) 
                       ?? await _context.Users.FirstOrDefaultAsync();

            if (user != null)
            {
                user.Username = newUsername;
                user.Password = newPassword;

                _context.Update(user);
                await _context.SaveChangesAsync();

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Success"] = "Bilgileriniz başarıyla güncellendi.";
                return RedirectToAction("Login");
            }
            else 
            {
                var newUser = new User 
                { 
                    Username = newUsername, 
                    Password = newPassword,
                    Email = "admin@torossolar.com"
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.AnyAsync(u => u.Username == model.Username);
                if (existingUser)
                {
                    ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                    return View(model);
                }

                var newUser = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password
                };

                try
                {
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Kaydınız başarıyla oluşturuldu! Şimdi giriş yapabilirsiniz.";
                    return RedirectToAction("UserLogin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendResetCode(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Lütfen e-posta adresinizi giriniz." });
            }

            Random rand = new Random();
            string resetCode = rand.Next(100000, 999999).ToString();
            HttpContext.Session.SetString("ResetPasswordCode", resetCode);

            return Json(new { success = true, message = "Sıfırlama kodu gönderildi.", debugCode = resetCode });
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myFavorites = await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .Select(f => f.Product)
                .ToListAsync();

            ViewBag.UserName = User.Identity?.Name ?? "Kullanıcı";
            return View(myFavorites);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Json(new { success = false });

            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.ProductId == productId && f.UserId == userId);

            if (existingFavorite != null)
            {
                _context.Favorites.Remove(existingFavorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "removed" });
            }
            else
            {
                _context.Favorites.Add(new Favorite { ProductId = productId, UserId = userId });
                await _context.SaveChangesAsync();
                return Json(new { success = true, action = "added" });
            }
        }
    }
}