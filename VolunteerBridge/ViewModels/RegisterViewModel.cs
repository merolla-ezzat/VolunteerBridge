using System.ComponentModel.DataAnnotations;
namespace VolunteerBridge.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")] 
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")] 
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "كلمة المرور يجب أن تتكون من 8 أحرف على الأقل وتتضمن حرفاً كبيراً، ورقم، ورمز خاص.")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها لا يتطابقان")] 
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
