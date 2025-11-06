using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}