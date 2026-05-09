using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;

namespace VolunteerBridge.Controllers
{
    public class AdminController : Controller
    {
        private const string AdminEmail = "yousef.ehab.k@gmail.com";
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        private IActionResult? ValidateAdminAccess()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = _db.Users.Find(userId.Value);
            if (user?.Email != AdminEmail) return RedirectToAction("Index", "Home");

            return null;
        }

        // Coded by Yousef: keep the admin dashboard read-only and simple for the project demo.
        public IActionResult Index()
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var vm = new AdminDashboardViewModel
            {
                TotalUsers = _db.Users.Count(),
                TotalRequests = _db.ServiceRequests.Count(),
                CompletedRequests = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Completed),
                OpenRequests = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Open),
                TotalPointsAwarded = _db.pointTransactions.Sum(t => (int?)t.Points) ?? 0,
                RecentUsers = _db.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(10)
                    .ToList(),
                RecentRequests = _db.ServiceRequests
                    .Include(r => r.Requester)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .ToList()
            };

            return View(vm);
        }

        public IActionResult Users()
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var users = _db.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(users);
        }

        public IActionResult Requests()
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var requests = _db.ServiceRequests
                .Include(r => r.Requester)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(requests);
        }
    }
}
