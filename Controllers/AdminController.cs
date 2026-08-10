using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagementSystem.Models;
using TodoManagementSystem.Services;

namespace TodoManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITodoService _todoService;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ITodoService todoService, UserManager<IdentityUser> userManager)
        {
            _todoService = todoService;
            _userManager = userManager;
        }

        // 1. ADMİN ANA SAYFASI (Listeleme)
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var allTodos = await _todoService.GetAllTodosAsync();

            ViewBag.AllTodos = allTodos;
            return View(users);
        }

        // 2. ADMİN İÇİN GÖREV DÜZENLEME EKRANI (GET)
        public async Task<IActionResult> EditTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null) return NotFound();
            
            return View(todo);
        }

        // 3. ADMİN İÇİN GÖREV DÜZENLEMEYİ KAYDETME (POST)
        [HttpPost]
        public async Task<IActionResult> EditTodo(TodoItem model)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                // Görevin veritabanındaki orijinal halini buluyoruz ki asıl sahibini değiştirmeyelim
                var existingTodo = await _todoService.GetTodoByIdAsync(model.Id);
                if (existingTodo != null)
                {
                    existingTodo.Title = model.Title;
                    existingTodo.Description = model.Description;
                    existingTodo.IsCompleted = model.IsCompleted;
                    
                    await _todoService.UpdateTodoAsync(existingTodo);
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // 4. ADMİN İÇİN GÖREV SİLME (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            await _todoService.DeleteTodoAsync(id);
            return RedirectToAction("Index");
        }
        // 5. ADMİN İÇİN KULLANICIYA GÖREV ATAMA EKRANI (GET)
        public async Task<IActionResult> AssignTodo()
        {
            // Kullanıcılara görev seçtirebilmek için tüm kullanıcıları listeye çekiyoruz
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View();
        }

        // 6. ADMİN İÇİN GÖREVİ KAYDETME (POST)
        [HttpPost]
        public async Task<IActionResult> AssignTodo(TodoItem model, string selectedUserId)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid && !string.IsNullOrEmpty(selectedUserId))
            {
                // Görevin sahibini, adminin formda seçtiği kullanıcının ID'si yapıyoruz
                model.UserId = selectedUserId;
                
                await _todoService.AddTodoAsync(model);
                return RedirectToAction("Index");
            }

            // Hata olursa kullanıcı listesini tekrar doldurup formu geri döndür
            ViewBag.Users = await _userManager.Users.ToListAsync();
            ModelState.AddModelError("", "Lütfen geçerli bir kullanıcı seçin.");
            return View(model);
        }
    }
}