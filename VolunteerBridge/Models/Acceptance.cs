using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.Models
{
    public class Acceptance
    {
        public int AcceptanceId { get; set; }
        [Display(Name = "الطلب")]
        public int RequestId { get; set; }
        public required ServiceRequest Request { get; set; }
        [Display(Name = "المتطوع")]
        public int VolunteerId { get; set; }
        public required User Volunteer { get; set; }
        [Display(Name = "تاريخ القبول")]
        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
        [Display(Name = "تاريخ الإكمال")]
        public DateTime? CompletedAt { get; set; }
        [Display(Name = "الحالة")]
        public Enums.AcceptanceStatus Status { get; set; } = Enums.AcceptanceStatus.Pending;
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
    }
}
