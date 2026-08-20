using TodoManagementSystem.Models;

namespace TodoManagementSystem.Services
{
    public interface ITodoService
    {
        // Kullanıcıya ait görevleri getirir
        Task<List<TodoItem>> GetTodosByUserIdAsync(string userId);
        
        // Admin için sistemdeki TÜM görevleri getirir
        Task<List<TodoItem>> GetAllTodosAsync();
        
        // ID'sine göre tek bir görevi getirir
        Task<TodoItem?> GetTodoByIdAsync(int id);
        
        // Yeni görev ekler
        Task AddTodoAsync(TodoItem todoItem);
        
        // Var olan görevi günceller
        Task UpdateTodoAsync(TodoItem todoItem);
        
        // Görevi siler
        Task DeleteTodoAsync(int id);
        //Yorum ekleme
        Task AddCommentAsync(TodoComment comment);
    }
}