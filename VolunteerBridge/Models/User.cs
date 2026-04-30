using System.ComponentModel.DataAnnotations;
using static VolunteerBridge.Models.Enums;

namespace VolunteerBridge.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Display(Name = "الاسم الكامل")]
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم الكامل يجب ألا يتجاوز 100 حرف")]
        public string FullName { get; set; } = string.Empty;
        [Display(Name = "البريد الإلكتروني")]
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [StringLength(150, ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 150 حرف")]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "كلمة المرور")]
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        [Display(Name = "رقم الهاتف")]
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب ألا يتجاوز 20 رقم/حرف")]

        public string PhoneNumber { get; set; } = string.Empty;
        [Display(Name = "المدينة")]
        [StringLength(100)]
        public string? City { get; set; }
        [Display(Name = "المهارات")]
        [StringLength(500)]
        public string? Skills { get; set; }
        [Display(Name = "نبذة شخصية")]
        [StringLength(1000)]
        public string? Bio { get; set; }
        [Display(Name = "عدد المهام المكتملة")]
        public int CompletedTasksCount { get; set; } = 0;
        [Display(Name = "إجمالي النقاط")]
        public int TotalPoints { get; set; } = 0;
        [Display(Name = "المستوى")]
        public UserLevel Level { get; set; } = UserLevel.Newcomer;
        [Display(Name = "متوسط التقييم")]
        [Range(0, 5, ErrorMessage = "متوسط التقييم يجب أن يكون بين 0 و 5")]
        public decimal AverageRating { get; set; } = 0;
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public ICollection<Acceptance> Acceptances { get; set; } = new List<Acceptance>();
        public ICollection<Rating> RatingsGiven { get; set; } = new List<Rating>();
        public ICollection<Rating> RatingsReceived { get; set; } = new List<Rating>();
        public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    }

}
