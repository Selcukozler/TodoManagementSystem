using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TodoManagementSystem.Data;
using TodoManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC ETKİNLEŞTİRME
builder.Services.AddControllersWithViews();

// VERİTABANI BAĞLANTISI
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// KULLANICI SİSTEMİNİN KURULMASI
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { 
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<TodoManagementSystem.Services.ITodoService, TodoManagementSystem.Services.TodoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 1. KRİTİK DÜZELTME: Sneat şablonu gibi harici statik dosyaların (wwwroot/sneat/...) 
// tarayıcı tarafından okunabilmesi için bu satır mutlaka Routing'den ÖNCE olmalıdır.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- ADMİN HESABI OLUŞTURMA KODLARI BAŞLANGICI ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminUser = await userManager.FindByEmailAsync("admin@mail.com");
    if (adminUser == null)
    {
        adminUser = new ApplicationUser { UserName = "admin@mail.com", Email = "admin@mail.com" };
        await userManager.CreateAsync(adminUser, "admin123"); 
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
// --- ADMİN HESABI OLUŞTURMA KODLARI BİTİŞİ ---

app.Run();