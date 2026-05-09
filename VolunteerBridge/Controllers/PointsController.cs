using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;

namespace VolunteerBridge.Controllers
{
    public class PointsController : Controller
    {
        private readonly AppDbContext _db;

        public PointsController(AppDbContext db)
        {
            _db = db;
        }

        // Coded by Yousef: show the logged-in user's points history using the existing point transactions table.
        public IActionResult History()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var transactions = _db.pointTransactions
                .Include(t => t.Acceptance)
                .ThenInclude(a => a.Request)
                .Where(t => t.UserId == userId.Value)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return View(transactions);
        }
    }
}
