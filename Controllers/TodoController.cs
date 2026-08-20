using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoManagementSystem.Models;
using TodoManagementSystem.Services;

namespace TodoManagementSystem.Controllers
{
    [Authorize] 
    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;
        // IdentityUser yerine ApplicationUser kullanıyoruz
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoService todoService, UserManager<ApplicationUser> userManager)
        {
            _todoService = todoService;
            _userManager = userManager;
        }

        // 1. GÖREVLERİ LİSTELEME EKRANI (Ana Sayfamız)
public async Task<IActionResult> Index()
{
    var userId = _userManager.GetUserId(User);
    
    // Servisteki "Tüm Görevleri Getir" metodunu kullanıyoruz (User bilgileri de dahil gelecek)
    var allTodos = await _todoService.GetAllTodosAsync();
    
    // Kendi görevlerim
    ViewBag.MyTodos = allTodos.Where(t => t.UserId == userId).ToList();
    
    // Diğer kişilerin görevleri
    ViewBag.OtherTodos = allTodos.Where(t => t.UserId != userId).ToList();
    
    return View(); // Artık doğrudan model göndermiyoruz, ViewBag kullanıyoruz
}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TodoItem todoItem)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                todoItem.UserId = _userManager.GetUserId(User)!;
                await _todoService.AddTodoAsync(todoItem);
                
                return RedirectToAction("Index"); 
            }
            
            return View(todoItem);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            if (todo.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized(); 
            }

            return View(todo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                todoItem.UserId = _userManager.GetUserId(User)!;
                await _todoService.UpdateTodoAsync(todoItem);
                
                return RedirectToAction("Index");
            }
            return View(todoItem);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            if (todo.UserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            
            if (todo != null && todo.UserId == _userManager.GetUserId(User))
            {
                await _todoService.DeleteTodoAsync(id);
            }
            
            return RedirectToAction("Index");
        }

        //YORUM YAPMA METODU
        [HttpPost]
        public async Task<IActionResult> AddComment(int todoId, string text)
{
    if (!string.IsNullOrWhiteSpace(text))
    {
        var comment = new TodoComment
        {
            TodoItemId = todoId,
            Text = text,
            UserId = _userManager.GetUserId(User)!
        };
        await _todoService.AddCommentAsync(comment);
    }
    
    // Yorum yapılan görev kimin?
    var todo = await _todoService.GetTodoByIdAsync(todoId);
    
    // Eğer görev benimse düzenleme (Edit) sayfasına geri dön
    if (todo != null && todo.UserId == _userManager.GetUserId(User))
    {
        return RedirectToAction("Edit", new { id = todoId }); 
    }
    
    // Eğer görev başkasınınsa detay (Details) sayfasına geri dön
    return RedirectToAction("Details", new { id = todoId }); 
}
    
    public async Task<IActionResult> Details(int id)
{
    var todo = await _todoService.GetTodoByIdAsync(id);
    if (todo == null) return NotFound();
    
    return View(todo);
}
    
    }
}