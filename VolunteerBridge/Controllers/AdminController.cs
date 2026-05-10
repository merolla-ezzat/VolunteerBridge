using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;

namespace VolunteerBridge.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public AdminController(AppDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        // Old ValidateAdminAccess modified by Yousef: now checks Session["IsAdmin"]
        private IActionResult? ValidateAdminAccess()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            var adminLoggedIn = HttpContext.Session.GetString("AdminLoggedIn");
            
            if (isAdmin != "true" || adminLoggedIn != "true") 
                return RedirectToAction("Login");
                
            return null;
        }

        // GET: /Admin
        // GET: /Admin/Login
        [Route("Admin")]
        [Route("Admin/Login")]
        public IActionResult Login()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            if (isAdmin == "true") return RedirectToAction("Dashboard");
            return View();
        }

        [HttpPost]
        [Route("Admin/Login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var adminEmail = _configuration["AdminSettings:Email"];
            var adminPassword = _configuration["AdminSettings:Password"];

            if (model.Email == adminEmail && model.Password == adminPassword)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "بيانات الدخول غير صحيحة");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("AdminLoggedIn");
            return RedirectToAction("Login");
        }

        // Coded by Yousef: keep the admin dashboard read-only and simple for the project demo.
        public IActionResult Dashboard()
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var vm = new AdminDashboardViewModel
            {
                TotalUsers = _db.Users.Count(),
                TotalRequests = _db.ServiceRequests.Count(),
                CompletedRequests = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Completed),
                OpenRequests = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Open),
                ActiveVolunteers = _db.Users.Count(u => u.IsActive && !u.IsBanned && u.TotalPoints > 0),
                BannedUsers = _db.Users.Count(u => u.IsBanned),
                TotalPointsAwarded = _db.pointTransactions.Sum(t => (int?)t.Points) ?? 0,
                
                TopVolunteers = _db.Users
                    .Where(u => !u.IsBanned)
                    .OrderByDescending(u => u.TotalPoints)
                    .ThenByDescending(u => u.AverageRating)
                    .ThenByDescending(u => u.CompletedTasksCount)
                    .Take(5)
                    .ToList(),

                RecentUsers = _db.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(10)
                    .ToList(),
                    
                RecentRequests = _db.ServiceRequests
                    .Include(r => r.Requester)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .ToList(),

                RecentCompletedTasks = _db.Acceptances
                    .Include(a => a.Request)
                    .Include(a => a.Volunteer)
                    .Where(a => a.Status == Enums.AcceptanceStatus.Done)
                    .OrderByDescending(a => a.CompletedAt)
                    .Take(10)
                    .ToList(),

                RecentRatings = _db.Ratings
                    .Include(r => r.FromUser)
                    .Include(r => r.ToUser)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .ToList()
            };

            return View(vm);
        }

        public IActionResult Users(int page = 1)
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            int pageSize = 15;
            var query = _db.Users.OrderByDescending(u => u.CreatedAt);
            
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var users = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BanUser(int id, string? reason)
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var user = _db.Users.Find(id);
            if (user != null && !user.IsBanned)
            {
                user.IsBanned = true;
                user.BanReason = reason;
                _db.SaveChanges();
                TempData["Success"] = "تم حظر المستخدم بنجاح.";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnbanUser(int id)
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var user = _db.Users.Find(id);
            if (user != null && user.IsBanned)
            {
                user.IsBanned = false;
                user.BanReason = null;
                _db.SaveChanges();
                TempData["Success"] = "تم رفع الحظر عن المستخدم بنجاح.";
            }

            return RedirectToAction("Users");
        }

        public IActionResult Requests(int page = 1)
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            int pageSize = 15;
            var query = _db.ServiceRequests
                .Include(r => r.Requester)
                .Include(r => r.Acceptances)
                .OrderByDescending(r => r.CreatedAt);
                
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var requests = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(requests);
        }

        // Modified by Yousef
        // Old code kept for reference
        // public IActionResult DeleteRequest(int id) { ... _db.ServiceRequests.Remove(request); ... }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveRequest(int id, string? reason)
        {
            var accessResult = ValidateAdminAccess();
            if (accessResult != null) return accessResult;

            var request = _db.ServiceRequests
                .Include(r => r.Acceptances)
                .FirstOrDefault(r => r.RequestId == id);

            if (request != null && !request.IsRemovedByAdmin)
            {
                if (request.Acceptances != null && request.Acceptances.Any())
                {
                    TempData["Error"] = "Cannot remove a request that already has volunteer participation.";
                    return RedirectToAction("Requests");
                }

                request.IsRemovedByAdmin = true;
                request.AdminRemovalReason = reason;
                _db.SaveChanges();
                TempData["Success"] = "تم إزالة الطلب بنجاح. سيتم إشعار صاحب الطلب وإخفاؤه عن المستخدمين.";
            }

            return RedirectToAction("Requests");
        }
    }
}
