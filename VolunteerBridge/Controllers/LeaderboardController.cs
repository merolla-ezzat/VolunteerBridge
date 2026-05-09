using Microsoft.AspNetCore.Mvc;
using VolunteerBridge.Models;

namespace VolunteerBridge.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly AppDbContext _db;

        public LeaderboardController(AppDbContext db)
        {
            _db = db;
        }

        // Coded by Yousef: display the top active volunteers based on earned points.
        public IActionResult Index()
        {
            var users = _db.Users
                .Where(u => u.IsActive && u.TotalPoints > 0)
                .OrderByDescending(u => u.TotalPoints)
                .ThenByDescending(u => u.CompletedTasksCount)
                .ThenBy(u => u.FullName)
                .Take(50)
                .ToList();

            return View(users);
        }
    }
}
