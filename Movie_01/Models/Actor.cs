using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الممثل مطلوب")]
        [StringLength(100, ErrorMessage = "اسم الممثل لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "اسم الممثل")]
        public string Name { get; set; }

        [Display(Name = "صورة الممثل")]
        public string? ProfilePicture { get; set; }

        [StringLength(1000)]
        [Display(Name = "السيرة الذاتية")]
        public string? Bio { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Navigation Property
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}