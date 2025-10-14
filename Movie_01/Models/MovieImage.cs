using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class MovieImage
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "رابط الصورة")]
        public string ImageUrl { get; set; }

        [StringLength(200)]
        [Display(Name = "وصف الصورة")]
        public string? Description { get; set; }

        // Foreign Key
        [Required]
        public int MovieId { get; set; }

        // Navigation Property
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
}