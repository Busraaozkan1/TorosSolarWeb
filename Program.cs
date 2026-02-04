using Microsoft.EntityFrameworkCore;
using TorosSolar.Data;

var builder = WebApplication.CreateBuilder(args);

// Servisleri ekle
builder.Services.AddControllersWithViews();

// SQLite Bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Geliştirme aşamasında hataları daha net görmek için
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Yerel çalışmada SSL hatalarını önlemek için geçici olarak kapatabilirsin
// app.UseHttpsRedirection(); 

// --- VİDEO VE STATİK DOSYA DESTEĞİ ---
var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
provider.Mappings[".mp4"] = "video/mp4";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();
app.UseAuthorization();

// --- YÖNLENDİRMELER ---
app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();