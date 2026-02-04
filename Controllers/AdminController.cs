using Microsoft.AspNetCore.Mvc;
using TorosSolar.Data;
using TorosSolar.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    public AdminController(ApplicationDbContext context) => _context = context;

    public IActionResult UrunEkle() => View();

    [HttpPost]
    public IActionResult UrunEkle(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }
}