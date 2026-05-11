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
                ModelState.AddModelError("Email", "هذا الإيميل مسجل مسبقًا");
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
            HttpContext.Session.SetString("PendingUser_FullName", model.FullName);
            HttpContext.Session.SetString("PendingUser_Email", model.Email);
            HttpContext.Session.SetString("PendingUser_Password", BCrypt.Net.BCrypt.HashPassword(model.Password));
            HttpContext.Session.SetString("PendingUser_City", model.City ?? "");
            HttpContext.Session.SetString("PendingUser_Phone", model.PhoneNumber ?? "");
            HttpContext.Session.SetString("PendingUser_Token", token);

            return RedirectToAction("CheckEmail");
        }



           
        // GET: /Account/CheckEmail
        public IActionResult CheckEmail()
        {
            return View();
        }
        // Confirm Email
        public IActionResult ConfirmEmail(string token)
        {
            // check if token matches the one in session 
            var sessionToken = HttpContext.Session.GetString("PendingUser_Token");
            if (sessionToken == null || sessionToken != token)
                return Content("رابط غير صحيح أو انتهت صلاحيته");

            // check if email already exists (in case someone registered with the same email while the user was confirming)
            var email = HttpContext.Session.GetString("PendingUser_Email")!;
            if (_db.Users.Any(u => u.Email == email))
                return Content("هذا الإيميل مسجل بالفعل");

            // add user to database
            var user = new User
            {
                FullName = HttpContext.Session.GetString("PendingUser_FullName")!,
                Email = email,
                PasswordHash = HttpContext.Session.GetString("PendingUser_Password")!,
                City = HttpContext.Session.GetString("PendingUser_City"),
                PhoneNumber = HttpContext.Session.GetString("PendingUser_Phone"),
                IsEmailConfirmed = true,
                EmailConfirmationToken = null
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            // delete session data
            HttpContext.Session.Remove("PendingUser_FullName");
            HttpContext.Session.Remove("PendingUser_Email");
            HttpContext.Session.Remove("PendingUser_Password");
            HttpContext.Session.Remove("PendingUser_City");
            HttpContext.Session.Remove("PendingUser_Phone");
            HttpContext.Session.Remove("PendingUser_Token");

            return RedirectToAction("CompleteProfile");
        }
        [HttpGet]

        public IActionResult CompleteProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            return View();
        }
        [HttpPost]

        public IActionResult CompleteProfile(string? bio, string? skills,string? experience)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _db.Users.Find(userId.Value);
            if (user == null) return RedirectToAction("Login");

            user.Bio = bio;
            user.Skills = skills;
            user.Experience = experience;
            _db.SaveChanges();

            return RedirectToAction("Index", "Home");
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
                ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
                return View(model);
            }
            
            // Youssef:Check if user is banned
            if (user.IsBanned)
            {
                ModelState.AddModelError("", $"تم إيقاف حسابك. {(string.IsNullOrEmpty(user.BanReason) ? "" : "السبب: " + user.BanReason)}");
                return View(model);
            }

            // no confirmation no login
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
            user.Bio = model.Bio;
            user.Skills = model.Skills;
            user.Experience = model.Experience;
            _db.SaveChanges();
            return RedirectToAction("Profile");
        }

        // youssef: to enable requester to show Volunteer's Profile
        public IActionResult ViewProfile(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

    }
}
