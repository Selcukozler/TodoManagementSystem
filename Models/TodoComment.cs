using System.ComponentModel.DataAnnotations;

namespace TodoManagementSystem.Models
{
    public class TodoComment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yorum boş olamaz.")]
        [StringLength(500)]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Hangi Todo'ya yapıldı?
        public int TodoItemId { get; set; }
        public virtual TodoItem TodoItem { get; set; } = null!;

        // Kim yaptı?
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}