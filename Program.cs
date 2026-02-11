using Microsoft.EntityFrameworkCore;
using TorosSolar.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC Servisleri
builder.Services.AddControllersWithViews();

// 2. SQLite Bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. KİMLİK DOĞRULAMA SERVİSİ (Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // ÖNEMLİ: Yetkisiz biri Product/Index'e girmeye çalışırsa 
        // buradaki adrese (yani senin giriş paneline) yönlendirilir.
        options.LoginPath = "/admin"; 
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/Index"; // Yetkisi olmayanı ana sayfaya at
        options.Cookie.Name = "TorosSolarAuth"; 
    });

// 4. SESSION (OTURUM) SERVİSİ
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Geliştirme modu ayarları
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Statik Dosya Desteği (Video ve Genel Dosyalar)
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mp4"] = "video/mp4";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

// 5. ARA KATMANLAR (Sıralama kritiktir!)
app.UseSession();         // Önce oturum
app.UseAuthentication();  // Sonra kimlik kontrolü (Sen kimsin?)
app.UseAuthorization();   // En son yetki kontrolü (Girebilir misin?)

// 6. YÖNLENDİRMELER (ROUTES)

// Controller üzerindeki [Route] özniteliklerini (örneğin [Route("admin")]) aktif eder
app.MapControllers(); 

// Özel Admin Rotası
app.MapControllerRoute(
    name: "admin",
    pattern: "admin",
    defaults: new { controller = "Account", action = "Login" });

// Varsayılan Rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();