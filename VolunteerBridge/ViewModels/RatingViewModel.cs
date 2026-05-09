using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.ViewModels
{
    public class RatingViewModel
    {
        public int AcceptanceId { get; set; }
        public int ToUserId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rate { get; set; }
        [StringLength(500)]
        public string? Comment { get; set; }
    }
}
