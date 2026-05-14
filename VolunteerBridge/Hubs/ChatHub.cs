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

        public override async Task OnConnectedAsync()
        {
            var userId = GetSessionUserId();
            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId.Value}");
            }
            await base.OnConnectedAsync();
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

            var currentUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId.Value);
            var peerExists = await db.Users.AnyAsync(u => u.UserId == otherUserId);
            if (!peerExists || currentUser == null)
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

            var payload = new
            {
                entity.Id,
                entity.SenderId,
                SenderName = currentUser.FullName, // Adding sender name for global notification context
                entity.ReceiverId,
                entity.Message,
                sentAt = entity.SentAt.ToString("O")
            };

            // Broadcast to the specific thread
            var group = ThreadGroupName(userId.Value, otherUserId);
            await Clients.Group(group).SendAsync("ReceiveMessage", payload);

            // Broadcast globally to the receiver so their floating widget can update (badge, mini-inbox)
            await Clients.Group($"user_{otherUserId}").SendAsync("ReceiveGlobalMessage", payload);
            
            // Also broadcast to the sender's global group to sync UI across multiple tabs
            await Clients.Group($"user_{userId.Value}").SendAsync("ReceiveGlobalMessage", payload);
        }
    }
}
