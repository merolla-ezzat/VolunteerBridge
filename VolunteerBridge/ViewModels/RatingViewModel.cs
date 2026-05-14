using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.ViewModels
{
    public class RatingViewModel
    {
        public int AcceptanceId { get; set; }
        public int ToUserId { get; set; }
        [Required(ErrorMessage = "معرف الطلب مطلوب")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rate { get; set; }
        [StringLength(500, ErrorMessage = "التعليق يجب ألا يتجاوز 500 حرف")]
        public string? Comment { get; set; }
    }
}
