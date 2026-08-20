using System.ComponentModel.DataAnnotations;

namespace TodoManagementSystem.Models
{
    public class TodoItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Title { get; set; } = string.Empty; 
        
        [StringLength(500)]
        public string? Description { get; set; } 

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Required]
        public string UserId { get; set; } = string.Empty; 
        
        // IdentityUser yerine kendi ApplicationUser sınıfımızı kullanıyoruz
        public virtual ApplicationUser User { get; set; } = null!; 

        // Bu Todo'ya ait yorumların listesi
        public virtual ICollection<TodoComment> Comments { get; set; } = new List<TodoComment>();
    }
}