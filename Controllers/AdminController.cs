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
        // IdentityUser yerine ApplicationUser kullanıyoruz
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ITodoService todoService, UserManager<ApplicationUser> userManager)
        {
            _todoService = todoService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var allTodos = await _todoService.GetAllTodosAsync();

            ViewBag.AllTodos = allTodos;
            return View(users);
        }

        public async Task<IActionResult> EditTodo(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null) return NotFound();
            
            return View(todo);
        }

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
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            await _todoService.DeleteTodoAsync(id);
            return RedirectToAction("Index");
        }

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
            ModelState.AddModelError("", "Lütfen geçerli bir kullanıcı seçin.");
            return View(model);
        }
    }
}