using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }
        [Display(Name = "عملية القبول")]
        public int AcceptanceId { get; set; }
        public required Acceptance Acceptance { get; set; }
        [Display(Name = "من المستخدم")]
        public int FromUserId { get; set; }
        public required User FromUser { get; set; }
        [Display(Name = "إلى المستخدم")]
        public int ToUserId { get; set; }
        public required User ToUser { get; set; }
        [Display(Name = "التقييم")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rate { get; set; }
        [Display(Name = "تعليق")]
        [StringLength(500)]
        public string? Comment { get; set; }
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
