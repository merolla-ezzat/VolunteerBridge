using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;

namespace VolunteerBridge.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ChatHub(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public static string ThreadGroupName(int userIdA, int userIdB)
        {
            var lo = Math.Min(userIdA, userIdB);
            var hi = Math.Max(userIdA, userIdB);
            return $"thread_{lo}_{hi}";
        }

        private int? GetSessionUserId()
        {
            return Context.GetHttpContext()?.Session.GetInt32("UserId");
        }

        public async Task JoinThread(int otherUserId)
        {
            var userId = GetSessionUserId();
            if (userId == null)
            {
                throw new HubException("يجب تسجيل الدخول للمحادثة.");
            }

            if (otherUserId == userId.Value)
            {
                throw new HubException("لا يمكن بدء محادثة مع نفسك.");
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var peerExists = await db.Users.AnyAsync(u => u.UserId == otherUserId);
            if (!peerExists)
            {
                throw new HubException("المستخدم غير موجود.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, ThreadGroupName(userId.Value, otherUserId));
        }

        public async Task SendMessage(int otherUserId, string message)
        {
            var userId = GetSessionUserId();
            if (userId == null)
            {
                throw new HubException("يجب تسجيل الدخول لإرسال الرسائل.");
            }

            if (otherUserId == userId.Value)
            {
                throw new HubException("لا يمكن إرسال رسالة إلى نفسك.");
            }

            message = (message ?? string.Empty).Trim();
            if (message.Length == 0)
            {
                return;
            }

            if (message.Length > 2000)
            {
                throw new HubException("الرسالة طويلة جداً (الحد الأقصى 2000 حرف).");
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var peerExists = await db.Users.AnyAsync(u => u.UserId == otherUserId);
            if (!peerExists)
            {
                throw new HubException("المستخدم غير موجود.");
            }

            var entity = new ChatMessage
            {
                SenderId = userId.Value,
                ReceiverId = otherUserId,
                Message = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            db.ChatMessages.Add(entity);
            await db.SaveChangesAsync();

            var group = ThreadGroupName(userId.Value, otherUserId);
            await Clients.Group(group).SendAsync("ReceiveMessage", new
            {
                entity.Id,
                entity.SenderId,
                entity.ReceiverId,
                entity.Message,
                sentAt = entity.SentAt.ToString("O")
            });
        }
    }
}
