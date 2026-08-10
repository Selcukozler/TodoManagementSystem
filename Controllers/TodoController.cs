using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoManagementSystem.Models;
using TodoManagementSystem.Services;

namespace TodoManagementSystem.Controllers
{
    // [Authorize] etiketi: "Bu sayfadaki hiçbir şeye giriş yapmamış biri ulaşamaz" demektir. Kapıya kilit vurduk.
    [Authorize] 
    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;
        private readonly UserManager<IdentityUser> _userManager;

        // Daha önce yazdığımız Servis'i (İş kurallarını) ve Kullanıcı Yöneticisi'ni içeri alıyoruz.
        public TodoController(ITodoService todoService, UserManager<IdentityUser> userManager)
        {
            _todoService = todoService;
            _userManager = userManager;
        }

        // 1. GÖREVLERİ LİSTELEME EKRANI (Ana Sayfamız)
        public async Task<IActionResult> Index()
        {
            // O an sisteme giriş yapmış olan kişinin kimlik numarasını (ID) buluyoruz
            var userId = _userManager.GetUserId(User);
            
            // SADECE o kişiye ait görevleri servisten istiyoruz
            var todos = await _todoService.GetTodosByUserIdAsync(userId!);
            
            return View(todos);
        }

        // 2. YENİ GÖREV EKLEME EKRANI (Açılış - GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. YENİ GÖREVİ VERİTABANINA KAYDETME (Gönderim - POST)
        [HttpPost]
        public async Task<IActionResult> Create(TodoItem todoItem)
        {
            // Sisteme diyoruz ki: "Formdan UserId ve User gelmeyecek, 
            // sen onlar için hata verme (validation yapma), ben onları aşağıda kendim dolduracağım."
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            // Şimdi kontrol et, Başlık vs. düzgün girilmiş mi?
            if (ModelState.IsValid)
            {
                // Görevin sahibini, o an giriş yapmış kişi olarak arka planda atıyoruz.
                todoItem.UserId = _userManager.GetUserId(User)!;
                
                // Servisi çağırıp veritabanına kaydet
                await _todoService.AddTodoAsync(todoItem);
                
                // Kayıt bitince listeleme (Index) sayfasına geri gönder
                return RedirectToAction("Index"); 
            }
            
            // Eğer model geçerli değilse (örneğin başlık boşsa) formu hatalarla geri döndür
            return View(todoItem);
        }

        // 4. GÖREV DÜZENLEME EKRANI (Açılış - GET)
        public async Task<IActionResult> Edit(int id)
        {
            // Tıklanan görevi veritabanından bul
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound(); // Görev yoksa hata sayfası göster
            }

            // GÜVENLİK KONTROLÜ: Bu görev bu kullanıcıya mı ait?
            if (todo.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized(); // Başkasının görevini düzenlemeye çalışırsa engelle
            }

            return View(todo);
        }

        // 5. GÖREV DÜZENLEMEYİ KAYDETME (Gönderim - POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            // Yine UserId ve User hatalarını görmezden gel diyoruz
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                // Güvenliği sağlamak için görevin sahibini tekrar atıyoruz
                todoItem.UserId = _userManager.GetUserId(User)!;

                // Servisi çağırıp değişiklikleri veritabanına kaydet
                await _todoService.UpdateTodoAsync(todoItem);
                
                return RedirectToAction("Index");
            }
            return View(todoItem);
        }
        // 6. GÖREV SİLME ONAY EKRANI (Açılış - GET)
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            // GÜVENLİK KONTROLÜ: Başkası senin görevini silemesin
            if (todo.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            return View(todo);
        }

        // 7. GÖREVİ VERİTABANINDAN SİLME (Gönderim - POST)
        [HttpPost, ActionName("Delete")] // HTML'deki form "Delete" ismini arayacağı için bu etiketi ekliyoruz
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            
            // Son bir güvenlik kontrolü daha yapıp siliyoruz
            if (todo != null && todo.UserId == _userManager.GetUserId(User))
            {
                await _todoService.DeleteTodoAsync(id);
            }
            
            return RedirectToAction("Index");
        }
    }
}