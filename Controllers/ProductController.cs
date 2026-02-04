using Microsoft.AspNetCore.Mvc;
using TorosSolar.Data;
using TorosSolar.Models;

namespace TorosSolar.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME: Ürünlerin listelendiği ana sayfa
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // 2. EKLEME (GET): Boş form sayfasını açar
        public IActionResult Create()
        {
            return View();
        }

        // 3. EKLEME (POST): Formdan gelen veriyi ve resmi kaydeder
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/img/" + fileName;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 4. DÜZENLEME (GET): Mevcut bilgileri forma doldurur
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // 5. DÜZENLEME (POST): Güncellenen verileri kaydeder
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/img/" + fileName;
            }
            else
            {
                // Yeni resim seçilmediyse eski resim yolunu korumak için takip et
                _context.Entry(product).Property(x => x.ImageUrl).IsModified = false;
            }

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 6. SİLME: Ürünü veritabanından kaldırır
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}