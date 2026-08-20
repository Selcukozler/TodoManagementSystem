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
//listeyi sıralama
        public async Task<List<TodoItem>> GetTodosByUserIdAsync(string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
//admin için bütün todoları getirme
        public async Task<List<TodoItem>> GetAllTodosAsync()
        {
            return await _context.TodoItems
                .Include(t => t.User) 
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
//todonun bağlı olduğu userIDyi getirme
       public async Task<TodoItem?> GetTodoByIdAsync(int id)
{
    return await _context.TodoItems
        .Include(t => t.User)
        .Include(t => t.Comments)         // Yorumları dahil et
            .ThenInclude(c => c.User)     // Yorumu yapan kullanıcıyı dahil et
        .FirstOrDefaultAsync(t => t.Id == id);
}
//YENİ TODO EKLEME
        public async Task AddTodoAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
        }
//TODO UPDATE
        public async Task UpdateTodoAsync(TodoItem todoItem)
        {
            _context.TodoItems.Update(todoItem);
            await _context.SaveChangesAsync();
        }
//TODO SİLME
        public async Task DeleteTodoAsync(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo != null)
            {
                _context.TodoItems.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }
          public async Task AddCommentAsync(TodoComment comment)
{
    _context.TodoComments.Add(comment);
    await _context.SaveChangesAsync();
}
    }
}