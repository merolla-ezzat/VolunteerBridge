using System.ComponentModel.DataAnnotations;
namespace VolunteerBridge.ViewModels
{
    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][MinLength(6)] public string Password { get; set; } = string.Empty;
        [Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
