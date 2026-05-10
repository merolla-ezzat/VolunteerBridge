using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;

namespace VolunteerBridge.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ServiceRequestsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Helper: get the logged-in user's ID from session
        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

       // GET: /ServiceRequests/Browse
            public IActionResult Browse(string? searchTerm, int? category, string? city)
        {
            // Modified by Yousef: add simple browse filters without changing the existing request listing flow.
            searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
            city = string.IsNullOrWhiteSpace(city) ? null : city.Trim();

            var query = _db.ServiceRequests
                .Include(r => r.Requester)
                .Where(r => r.Status == Enums.RequestStatus.Open && !r.IsRemovedByAdmin);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Title.Contains(searchTerm) || r.Description.Contains(searchTerm));
            }

            if (category.HasValue)
            {
                query = query.Where(r => (int)r.Category == category.Value);
            }

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(r => r.Location.Contains(city));
            }

            var vm = new BrowseFilterViewModel
            {
                Requests = query
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList(),
                SearchTerm = searchTerm,
                Category = category.HasValue ? (Enums.RequestCategory)category.Value : null,
                City = city
            };

            return View(vm);
        }

        // GET: /ServiceRequests/MyRequests
        public IActionResult MyRequests()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var requests = _db.ServiceRequests
                .Where(r => r.RequesterId == userId && (!r.IsRemovedByAdmin || !r.RemovalAcknowledged))
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(requests);
        }

        // GET: /ServiceRequests/Create
        public IActionResult Create()
        {
            if (GetUserId() == null) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: /ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            // رفع الصورة لو موجودة
            string? imagePath = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                // تأكد إن الملف صورة
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(model.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Image", "يرجى رفع صورة بصيغة jpg, png, أو gif فقط");
                    return View(model);
                }

                // عمل اسم فريد للصورة
                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                // عمل الفولدر لو مش موجود
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            var request = new ServiceRequest
            {
                RequesterId = userId.Value,
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                Category = model.Category,
                ScheduledDate = model.ScheduledDate,
                EstimatedHours = model.EstimatedHours,
                PointsReward = Math.Max(10, (int)(model.EstimatedHours * 20)),
                Status = Enums.RequestStatus.Open,
                ImagePath = imagePath
            };

            _db.ServiceRequests.Add(request);
            _db.SaveChanges();

            TempData["Success"] = "تم نشر طلبك بنجاح!";
            return RedirectToAction("MyRequests");
        }

        // GET: /ServiceRequests/Details/5
        public IActionResult Details(int id)
        {
            var request = _db.ServiceRequests
                .Include(r => r.Requester)
                .Include(r => r.Acceptances)
                   .ThenInclude(a => a.Volunteer)
                .Include(r => r.Acceptances)
                  .ThenInclude(a => a.Ratings)
                .FirstOrDefault(r => r.RequestId == id);

            if (request == null) return NotFound();
            return View(request);
        }

        // POST: /ServiceRequests/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var request = _db.ServiceRequests.Find(id);
            if (request == null) return NotFound();

            if (request.RequesterId != userId) return Forbid();

            if (request.Status == Enums.RequestStatus.Open)
            {
                request.Status = Enums.RequestStatus.Cancelled;
                _db.SaveChanges();
                TempData["Success"] = "تم إلغاء الطلب.";
            }

            return RedirectToAction("MyRequests");
        }
        // POST: /ServiceRequests/AcknowledgeRemoval/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AcknowledgeRemoval(int id)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("Login", "Account");

            var request = _db.ServiceRequests.Find(id);
            if (request == null) return NotFound();

            if (request.RequesterId != userId) return Forbid();

            if (request.IsRemovedByAdmin && !request.RemovalAcknowledged)
            {
                request.RemovalAcknowledged = true;
                _db.SaveChanges();
            }

            return RedirectToAction("MyRequests");
        }
    }
}
