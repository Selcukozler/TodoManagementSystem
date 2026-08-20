using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoManagementSystem.Models;

namespace TodoManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoComment> TodoComments { get; set; }

        // EKLEDİĞİMİZ YENİ KISIM:
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tabloları için bu satır şart!

            // Çakışmayı önlemek için Kullanıcı -> Yorum ilişkisinde otomatik silmeyi (Cascade) kapatıyoruz
            builder.Entity<TodoComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}