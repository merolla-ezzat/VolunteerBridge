using System.ComponentModel.DataAnnotations;
using static VolunteerBridge.Models.Enums;

namespace VolunteerBridge.Models
{
    public class ServiceRequest
    {
        [Key]
        public int RequestId { get; set; }
        [Display(Name = "مقدم الطلب")]
        [Required(ErrorMessage = "مقدم الطلب مطلوب")]
        public int RequesterId { get; set; }
        public virtual User? Requester { get; set; }
        
        [Display(Name = "العنوان")]
        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(150, ErrorMessage = "العنوان يجب ألا يتجاوز 150 حرف")]
        public string Title { get; set; } = string.Empty;
        [Display(Name = "التصنيف")]
        [Required(ErrorMessage = "اختر التصنيف")]
        public Enums.RequestCategory Category { get; set; }
        [Display(Name = "الوصف")]
        [Required(ErrorMessage = "الوصف مطلوب")]
        public string Description { get; set; } = string.Empty;
        [Display(Name = "الموقع")]
        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(200, ErrorMessage = "الموقع يجب ألا يتجاوز 200 حرف")]
        public string Location { get; set; } = string.Empty;
        [Display(Name = "التاريخ والوقت")]
        public DateTime ScheduledDate { get; set; }
        [Display(Name = "الساعات التقديرية")]
        [Range(0.1, 999.9, ErrorMessage = "الساعات التقديرية غير صحيحة")]
        public decimal EstimatedHours { get; set; }
        [Display(Name = "الحالة")]
        public RequestStatus Status { get; set; } = RequestStatus.Open;
        [Display(Name = "نقاط المكافأة")]
        public int PointsReward { get; set; }
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImagePath { get; set; }
        public ICollection<Acceptance> Acceptances { get; set; } = new List<Acceptance>();

        // Admin Removal / Moderation fields (Modified by Yousef)
        public bool IsRemovedByAdmin { get; set; }
        public string? AdminRemovalReason { get; set; }
        public bool RemovalAcknowledged { get; set; }
    }
}
