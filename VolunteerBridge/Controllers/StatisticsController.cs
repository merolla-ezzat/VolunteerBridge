using Microsoft.AspNetCore.Mvc;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;

namespace VolunteerBridge.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _db;

        public StatisticsController(AppDbContext db)
        {
            _db = db;
        }

        // Coded by Yousef: provide a statistics page for Admins only.
        public IActionResult Index()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            var adminLoggedIn = HttpContext.Session.GetString("AdminLoggedIn");

            if (isAdmin != "true" || adminLoggedIn != "true")
                return RedirectToAction("Login", "Admin");

            var vm = new StatisticsViewModel
            {
                TotalUsers = _db.Users.Count(u => u.IsActive),
                TotalRequests = _db.ServiceRequests.Count(),
                TotalOpenRequests = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Open),
                TotalCompletedRequests = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Completed),
                TotalVolunteers = _db.Acceptances.Select(a => a.VolunteerId).Distinct().Count(),
                TotalPointsAwarded = _db.pointTransactions.Sum(t => (int?)t.Points) ?? 0,
                TopCity = _db.Users
                    .Where(u => u.IsActive && !string.IsNullOrWhiteSpace(u.City))
                    .GroupBy(u => u.City)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            };

            return View(vm);
        }
    }
}
