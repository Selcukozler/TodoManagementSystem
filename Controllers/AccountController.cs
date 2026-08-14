using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TodoManagementSystem.ViewModels;

namespace TodoManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        // Identity paketinin bize sunduğu hazır kullanıcı ve giriş yöneticileri
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // 1. KAYIT OLMA EKRANINI AÇAN METOT (GET)
        public IActionResult Register()
        {
            return View();
        }

        // 2. KAYIT OLMA BUTONUNA BASILINCA ÇALIŞAN METOT (POST)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Kullanıcı ViewModel'deki kurallara (boş bırakmama, şifre eşleşmesi vs.) uydu mu?
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Kayıt başarılıysa kullanıcıyı otomatik içeri al (Giriş yap)
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendir
                }

                // Eğer şifre zayıfsa vb. hatalar varsa bunları ekrana gönder
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // 3. GİRİŞ YAPMA EKRANINI AÇAN METOT (GET)
        public IActionResult Login()
        {
            return View();
        }

        // 4. GİRİŞ YAP BUTONUNA BASILINCA ÇALIŞAN METOT (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // E-posta ve şifreyi kontrol et

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home"); // Başarılıysa ana sayfaya git
                }

                // Hatalıysa genel bir hata mesajı ver
                ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            }
            return View(model);
        }

        // 5. ÇIKIŞ YAPMA METODU (POST)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Çıkış yapınca ana sayfaya dön
        }
    }
}