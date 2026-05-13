using System.ComponentModel.DataAnnotations;
namespace VolunteerBridge.ViewModels
{
    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "كلمة المرور يجب أن تتكون من 8 أحرف على الأقل وتتضمن حرفاً كبيراً، ورقم، ورمز خاص.")]
        public string Password { get; set; } = string.Empty;
        [Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
