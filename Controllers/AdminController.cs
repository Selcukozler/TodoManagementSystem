using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoManagementSystem.Data; // DbContext için gerekli
using TodoManagementSystem.Models;
using TodoManagementSystem.Services;

namespace TodoManagementSystem.Controllers
{
    [Authorize] // Admin kontrolünü Login'de hallettiğimiz için burada Authorize yeterli
    public class AdminController : Controller
    {
        private readonly ITodoService _todoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // Yorum silmek için ekledik

        public AdminController(ITodoService todoService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _todoService = todoService;
            _userManager = userManager;
            _context = context;
        }

        // 1. SİSTEM ÖZETİ (Anasayfa)
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.AllTodos = await _todoService.GetAllTodosAsync();
            return View(users);
        }

        // 2. KULLANICI YÖNETİMİ
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // 3. KULLANICI SİLME
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Users");
        }

        // 4. TÜM GÖREVLER EKRANI
        public async Task<IActionResult> AllTodos()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return View(todos);
        }

        // 5. GÖREV SİLME
        [HttpPost]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            await _todoService.DeleteTodoAsync(id);
            return RedirectToAction("AllTodos");
        }

        // 6. GÖREV DÜZENLEME (GET)
        public async Task<IActionResult> EditTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null) return NotFound();
            return View(todo);
        }

        // 7. GÖREV DÜZENLEME (POST)
        [HttpPost]
        public async Task<IActionResult> EditTodo(TodoItem model)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var existingTodo = await _todoService.GetTodoByIdAsync(model.Id);
                if (existingTodo != null)
                {
                    existingTodo.Title = model.Title;
                    existingTodo.Description = model.Description;
                    existingTodo.IsCompleted = model.IsCompleted;
                    await _todoService.UpdateTodoAsync(existingTodo);
                }
                return RedirectToAction("AllTodos");
            }
            return View(model);
        }

        // 8. YORUM SİLME
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId, int todoId)
        {
            // 'Comments' yerine senin kullandığın 'TodoComments' ismini yazdık
            var comment = await _context.TodoComments.FindAsync(commentId);
            if (comment != null)
            {
                _context.TodoComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            // Sildikten sonra tekrar görev düzenleme sayfasına dön
            return RedirectToAction("EditTodo", new { id = todoId });
        }

        // 9. GÖREV ATAMA (Mevcut kodun)
        public async Task<IActionResult> AssignTodo()
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignTodo(TodoItem model, string selectedUserId)
        {
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            if (ModelState.IsValid && !string.IsNullOrEmpty(selectedUserId))
            {
                model.UserId = selectedUserId;
                await _todoService.AddTodoAsync(model);
                return RedirectToAction("Index");
            }
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View(model);
        }
    }
}