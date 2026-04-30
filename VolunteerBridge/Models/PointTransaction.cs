using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.Models
{
    public class PointTransaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Display(Name = "المستخدم")]
        public int UserId { get; set; }
        public required User User { get; set; }
        [Display(Name = "النقاط")]
        [Range(1, int.MaxValue, ErrorMessage = "النقاط يجب أن تكون أكبر من صفر")]
        public int Points { get; set; }
        [Display(Name = "السبب")]
        [StringLength(200)]
        public string? Reason { get; set; }
        [Display(Name = "عملية القبول")]
        public int? AcceptanceId { get; set; }
        public Acceptance? Acceptance { get; set; }
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
