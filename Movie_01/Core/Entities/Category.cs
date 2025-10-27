using System.ComponentModel.DataAnnotations;

namespace MovieApp.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم التصنيف مطلوب")]
        [StringLength(100, ErrorMessage = "اسم التصنيف لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "اسم التصنيف")]
        public string Name { get; set; }

        [Display(Name = "صورة التصنيف")]
        public string? ImageUrl { get; set; }

        // Navigation Property
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}