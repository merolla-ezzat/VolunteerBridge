using System.ComponentModel.DataAnnotations;

namespace VolunteerBridge.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "الرسالة مطلوبة")]
        [StringLength(2000, ErrorMessage = "يجب ألا تتجاوز الرسالة 2000 حرف")]
        public string Message { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; }

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
    }
}
