using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using VolunteerBridge.Models;

namespace VolunteerBridge.ViewModels
{
    public class CreateRequestViewModel
    {
        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(150, ErrorMessage = "العنوان يجب ألا يتجاوز 150 حرف")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "الوصف مطلوب")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(200, ErrorMessage = "الموقع يجب ألا يتجاوز 200 حرف")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "الفئة مطلوبة")]
        public Enums.RequestCategory Category { get; set; }

        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateTime ScheduledDate { get; set; }

        [Required(ErrorMessage = "عدد الساعات مطلوب")]
        [Range(0.5, 24, ErrorMessage = "يجب أن تكون الساعات بين 0.5 و 24")]
        public decimal EstimatedHours { get; set; }

        // الصورة اختيارية
        public IFormFile? Image { get; set; }
    }
}
