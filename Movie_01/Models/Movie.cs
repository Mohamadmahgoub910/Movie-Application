using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public enum MovieStatus
    {
        [Display(Name = "قريباً")]
        ComingSoon,
        [Display(Name = "يُعرض الآن")]
        NowShowing,
        [Display(Name = "انتهى العرض")]
        Ended
    }

    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفيلم مطلوب")]
        [StringLength(200, ErrorMessage = "اسم الفيلم لا يمكن أن يتجاوز 200 حرف")]
        [Display(Name = "اسم الفيلم")]
        public string Name { get; set; }

        [Required(ErrorMessage = "الوصف مطلوب")]
        [StringLength(2000, ErrorMessage = "الوصف لا يمكن أن يتجاوز 2000 حرف")]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Display(Name = "السعر")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000, ErrorMessage = "السعر يجب أن يكون بين 0 و 10000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الحالة مطلوبة")]
        [Display(Name = "الحالة")]
        public MovieStatus Status { get; set; }

        [Required(ErrorMessage = "تاريخ العرض مطلوب")]
        [Display(Name = "تاريخ العرض")]
        [DataType(DataType.DateTime)]
        public DateTime ReleaseDateTime { get; set; }

        [Display(Name = "الصورة الرئيسية")]
        public string? MainImage { get; set; }

        [Display(Name = "مدة الفيلم (بالدقائق)")]
        [Range(1, 500, ErrorMessage = "مدة الفيلم يجب أن تكون بين 1 و 500 دقيقة")]
        public int? Duration { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "التصنيف مطلوب")]
        [Display(Name = "التصنيف")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "السينما مطلوبة")]
        [Display(Name = "السينما")]
        public int CinemaId { get; set; }

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("CinemaId")]
        public Cinema? Cinema { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public ICollection<MovieImage> SubImages { get; set; } = new List<MovieImage>();
    }
}