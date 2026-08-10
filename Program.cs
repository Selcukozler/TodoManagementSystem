using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TodoManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// BURASI GÜNCELLENDİ: AddDefaultIdentity yerine AddIdentity kullandık
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    
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
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// --- ADMİN HESABI OLUŞTURMA KODLARI BAŞLANGICI ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1. "Admin" adında bir rol var mı kontrol et, yoksa oluştur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2. Sistemde patron (admin) hesabı var mı kontrol et
    var adminUser = await userManager.FindByEmailAsync("admin@mail.com");
    if (adminUser == null)
    {
        // Yoksa yeni bir patron hesabı oluştur
        adminUser = new IdentityUser { UserName = "admin@mail.com", Email = "admin@mail.com" };
        
        // Şifresini "admin123" olarak belirliyoruz
        await userManager.CreateAsync(adminUser, "admin123"); 

        // Bu kullanıcıya "Admin" rozetini (rolünü) tak
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
// --- ADMİN HESABI OLUŞTURMA KODLARI BİTİŞİ ---

app.Run(); // Bu satır zaten dosyanın en sonunda vardı s