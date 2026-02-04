using Microsoft.AspNetCore.Mvc;
using TorosSolar.Data;
using TorosSolar.Models;
using System.Diagnostics;

namespace TorosSolar.Controllers
{
    public class HomeController : Controller
    {
        // Veritabanı bağlantısı için gerekli değişken
        private readonly ApplicationDbContext _context;

        // Constructor: Veritabanı bağlamını (Context) içeri alıyoruz
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Veritabanından en yeni eklenen 6 ürünü çekiyoruz (Grid yapısına uygun olsun diye 6 idealdir)
            var anaSayfaUrunleri = _context.Products
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToList();

            return View(anaSayfaUrunleri);
        }

        // Klasöründeki 'Urunler.cshtml' dosyası ile eşleşen metot
        public IActionResult Urunler()
        {
            // Veritabanındaki tüm ürünleri liste halinde çekiyoruz
            var tumUrunler = _context.Products
                .OrderByDescending(p => p.Id)
                .ToList();

            return View(tumUrunler);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}