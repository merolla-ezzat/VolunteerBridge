using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.Models
{
    public class Enums
    {
        public enum UserLevel
        {
            [Display(Name = "مبتدئ الخير")]
            Newcomer = 0,
            [Display(Name = "صانع الفرق")]
            Helper = 1,
            [Display(Name = "موثوق العطاء")]
            Trusted = 2,
            [Display(Name = "بطل المجتمع")]
            Champion = 3
        }
        public enum RequestStatus
        {
            [Display(Name = "مفتوح")]
            Open = 0,
            [Display(Name = "تم القبول")]
            Accepted = 1,
            [Display(Name = "مكتمل")]
            Completed = 2,
            [Display(Name = "ملغي")]
            Cancelled = 3
        }
        public enum AcceptanceStatus
        {
            [Display(Name = "قيد الانتظار")]
            Pending = 0,
            [Display(Name = "قيد التنفيذ")]
            InProgress = 1,
            [Display(Name = "تم الإنجاز")]
            Done = 2,
            [Display(Name = "ملغي")]
            Cancelled = 3
        }
        public enum RequestCategory
        {
            [Display(Name = "مجتمعي")]
            Community = 0,
            [Display(Name = "تعليمي")]
            Education = 1,
            [Display(Name = "تقني")]
            Tech = 2,
            [Display(Name = "صحي")]
            Healthcare = 3,
            [Display(Name = "بيئي")]
            Environment = 4,
            [Display(Name = "فعاليات")]
            Events = 5,
            [Display(Name = "إنساني")]
            Humanitarian = 6,
            [Display(Name = "إرشاد")]
            Mentorship = 7,
            [Display(Name = "رعاية الحيوانات")]
            AnimalWelfare = 8 ,
            [Display(Name = "أخرى")]
            Other = 9
        }

    }
}
