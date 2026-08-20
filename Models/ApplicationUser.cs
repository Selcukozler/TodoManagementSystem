using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TodoManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePicturePath { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [StringLength(300)]
        public string? Bio { get; set; }
    }
}