 using Microsoft.EntityFrameworkCore;
using TodoManagementSystem.Data;
using TodoManagementSystem.Models;

namespace TodoManagementSystem.Services
{
    public class TodoService : ITodoService
    {
        private readonly ApplicationDbContext _context;

        public TodoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TodoItem>> GetTodosByUserIdAsync(string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            return await _context.TodoItems
                .Include(t => t.User) 
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TodoItem?> GetTodoByIdAsync(int id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task AddTodoAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTodoAsync(TodoItem todoItem)
        {
            _context.TodoItems.Update(todoItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoAsync(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null)
            {
                _context.TodoItems.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }
    }
}