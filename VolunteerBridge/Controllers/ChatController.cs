using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;

namespace VolunteerBridge.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _db;

        public ChatController(AppDbContext db)
        {
            _db = db;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

        private async Task<List<ChatInboxRowViewModel>> GetInboxRowsAsync(int userId)
        {
            const int take = 2000;
            var messages = await _db.ChatMessages.AsNoTracking()
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .Take(take)
                .ToListAsync();

            var partnerIds = messages
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToList();

            if (partnerIds.Count == 0) return new List<ChatInboxRowViewModel>();

            var names = await _db.Users.AsNoTracking()
                .Where(u => partnerIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId, u => u.FullName);

            var rows = new List<ChatInboxRowViewModel>();
            foreach (var pid in partnerIds)
            {
                var conv = messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == pid) || (m.SenderId == pid && m.ReceiverId == userId))
                    .OrderByDescending(m => m.SentAt)
                    .ToList();
                if (conv.Count == 0) continue;

                var last = conv[0];
                var unread = conv.Count(m => m.SenderId == pid && m.ReceiverId == userId && !m.IsRead);
                var preview = last.Message.Length > 120 ? last.Message[..120] + "…" : last.Message;

                rows.Add(new ChatInboxRowViewModel
                {
                    OtherUserId = pid,
                    OtherUserName = names.TryGetValue(pid, out var n) ? n : "مستخدم",
                    LastMessageAt = last.SentAt,
                    LastMessagePreview = preview,
                    UnreadCount = unread
                });
            }

            return rows.OrderByDescending(r => r.LastMessageAt).ToList();
        }



        // GET: /Chat/History?otherUserId=5
        [HttpGet]
        public async Task<IActionResult> History(int otherUserId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            if (otherUserId == userId.Value)
            {
                return BadRequest();
            }

            var peerExists = await _db.Users.AnyAsync(u => u.UserId == otherUserId);
            if (!peerExists)
            {
                return NotFound();
            }

            var messages = await _db.ChatMessages
                .AsNoTracking()
                .Where(m =>
                    (m.SenderId == userId.Value && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId.Value))
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.Message,
                    sentAt = m.SentAt
                })
                .ToListAsync();

            return new JsonResult(messages, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        private async Task MarkConversationAsReadAsync(int userId, int otherUserId)
        {
            await _db.ChatMessages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId && !m.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentConversations()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var rows = await GetInboxRowsAsync(userId.Value);
            return new JsonResult(rows, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            if (userId == null) return Json(new { unreadCount = 0 });

            var count = await _db.ChatMessages
                .CountAsync(m => m.ReceiverId == userId.Value && !m.IsRead);

            return Json(new { unreadCount = count });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int otherUserId)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            await MarkConversationAsReadAsync(userId.Value, otherUserId);
            return Ok();
        }
    }
}
