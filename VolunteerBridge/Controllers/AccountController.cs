using Microsoft.AspNetCore.Mvc;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;
using VolunteerBridge.Services;
using BCrypt.Net;
using System.Threading.Tasks;

namespace VolunteerBridge.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;

        }

        // Register
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_db.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already in use");
                return View(model);
            }

            var token = Guid.NewGuid().ToString();

            // try to  confirma email
            var confirmLink = Url.Action("ConfirmEmail", "Account",
            new { token = token }, Request.Scheme);
            try
            {
                await _emailService.SendConfirmationEmailAsync(model.Email, model.FullName, confirmLink!);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "فشل إرسال بريد التأكيد. تأكد من صحة الإيميل وحاول مرة أخرى.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                City = model.City,
                PhoneNumber = model.PhoneNumber,
                IsEmailConfirmed = false,
                EmailConfirmationToken = token
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return RedirectToAction("CheckEmail");
        }
        //login
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }
            //  no confirmation no login
            if (!user.IsEmailConfirmed)
            {
                ModelState.AddModelError("", "من فضلك أكّد بريدك الإلكتروني الأول قبل تسجيل الدخول.");
                return View(model);
            }
            // save user ID in session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            return RedirectToAction("Index", "Home");
        }
        // logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        //  get profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _db.Users.Find(userId.Value);
            return View(user);
        }
        // GET: /Account/EditProfile
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _db.Users.Find(userId.Value);
            return View(user);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        public IActionResult EditProfile(User model)
        {
            var user = _db.Users.Find(model.UserId);
            if (user == null) return RedirectToAction("Login");

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.City = model.City;
            user.PhoneNumber = model.PhoneNumber;
            _db.SaveChanges();
            return RedirectToAction("Profile");
        }

      
        public IActionResult ConfirmEmail(string token)
        {
            var user = _db.Users.FirstOrDefault(u => u.EmailConfirmationToken == token);
            if (user == null) return Content("رابط غير صحيح");

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        // GET: /Account/CheckEmail
        public IActionResult CheckEmail()
        {
            return View();
        }

    }
}
