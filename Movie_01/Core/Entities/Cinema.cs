using System.ComponentModel.DataAnnotations;

namespace MovieApp.Core.Entities
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم السينما مطلوب")]
        [StringLength(100, ErrorMessage = "اسم السينما لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "اسم السينما")]
        public string Name { get; set; }

        [Display(Name = "شعار السينما")]
        public string? Logo { get; set; }

        [StringLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [StringLength(200)]
        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        // Navigation Property
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}