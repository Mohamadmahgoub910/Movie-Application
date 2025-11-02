using System.ComponentModel.DataAnnotations;
using MovieApp.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace MovieApp.Core.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفيلم مطلوب")]
        [StringLength(200)]
        [Display(Name = "اسم الفيلم")]
        public string Name { get; set; }

        [Required(ErrorMessage = "الوصف مطلوب")]
        [StringLength(2000)]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Display(Name = "السعر")]
        [Range(0, 10000, ErrorMessage = "السعر يجب أن يكون بين 0 و 10000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الحالة مطلوبة")]
        [Display(Name = "الحالة")]
        public MovieStatus Status { get; set; }

        [Required(ErrorMessage = "تاريخ العرض مطلوب")]
        [Display(Name = "تاريخ العرض")]
        [DataType(DataType.DateTime)]
        public DateTime ReleaseDateTime { get; set; }

        [Display(Name = "مدة الفيلم (بالدقائق)")]
        [Range(1, 500)]
        public int? Duration { get; set; }

        [Required(ErrorMessage = "التصنيف مطلوب")]
        [Display(Name = "التصنيف")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "السينما مطلوبة")]
        [Display(Name = "السينما")]
        public int CinemaId { get; set; }

        [Display(Name = "الصورة الرئيسية")]
        public IFormFile? MainImageFile { get; set; }

        public string? MainImage { get; set; }

        [Display(Name = "الممثلين")]
        public List<int>? SelectedActorIds { get; set; }

        [Display(Name = "صور إضافية")]
        public List<IFormFile>? SubImageFiles { get; set; }

        public List<MovieImage>? ExistingSubImages { get; set; }
    }
}