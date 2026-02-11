using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Güvenlik için gerekli kütüphane
using TorosSolar.Data;
using TorosSolar.Models;

namespace TorosSolar.Controllers
{
    // [Authorize] özniteliği sayesinde, sisteme giriş yapmamış hiç kimse 
    // doğrudan URL yazarak (Product/Index gibi) bu işlemlere erişemez.
    [Authorize] 
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME: Admin panelinde ürünlerin listelendiği ana sayfa
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // 2. DETAY: Tek bir ürünün bilgilerini getirir
        // Not: Bu sayfa hem admin panelinde hem de kullanıcı tarafında izlenebilir.
        // Eğer kullanıcıların giriş yapmadan detay görmesini istiyorsan bu metodun üzerine [AllowAnonymous] ekleyebilirsin.
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // 3. EKLEME (GET): Boş form sayfasını açar
        public IActionResult Create()
        {
            return View();
        }

        // 4. EKLEME (POST): Formdan gelen veriyi ve resmi kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
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

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // 5. DÜZENLEME (GET): Mevcut bilgileri forma doldurur
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // 6. DÜZENLEME (POST): Güncellenen verileri kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
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
                        // Resim yüklenmediyse mevcut resmi korumak için takip et
                        _context.Entry(product).Property(x => x.ImageUrl).IsModified = false;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // 7. SİLME: Ürünü veritabanından kaldırır
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}