// Controllers/RequestProgressController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;
using static VolunteerBridge.Models.Enums;

namespace VolunteerBridge.Controllers
{
    public class RequestProgressController : Controller
    {
        private readonly AppDbContext _db;

        public RequestProgressController(AppDbContext db)
        {
            _db = db;
        }

        // ─────────────────────────────────────────────────────────
        // PAGE: /RequestProgress/Index/5
        // ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Index(int id)
        {
            // Step 1: Make sure the user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Step 2: Load the request from the database
            var request = await _db.ServiceRequests
                .Include(r => r.Requester)
                .Include(r => r.Acceptances)
                    .ThenInclude(a => a.Volunteer)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            // Step 3: Find the active acceptance (skip cancelled ones)
            Acceptance? acceptance = null;
            foreach (var a in request.Acceptances)
            {
                if (a.Status != AcceptanceStatus.Cancelled)
                {
                    acceptance = a;
                    break;
                }
            }

            // Step 4: Check who is viewing the page
            bool isRequester = (request.RequesterId == userId);
            bool isVolunteer = (acceptance != null && acceptance.VolunteerId == userId);

            if (!isRequester && !isVolunteer)
                return Forbid();

            // Step 5: Check if a rating exists
            Rating? rating = null;
            if (acceptance != null)
            {
                rating = await _db.Ratings
                    .FirstOrDefaultAsync(r => r.AcceptanceId == acceptance.AcceptanceId);
            }

            // Step 6: Figure out the current stage
            int stage = GetCurrentStage(request, acceptance, rating);

            // Step 7: Arabic and English labels
            string[] arabicLabels = {
                "",
                "تم إنشاء الطلب",        // 1
                "انتظار متطوع",           // 2
                "تم قبول المتطوع",        // 3
                "العمل جارٍ",             // 4
                "انتظار تأكيد الإتمام",  // 5
                "مكتمل",                  // 6
                "تم التقييم"              // 7
            };

            string[] englishLabels = {
                "",
                "Request Created",        // 1
                "Waiting for Volunteer",  // 2
                "Volunteer Accepted",     // 3
                "Task In Progress",       // 4
                "Waiting for Completion", // 5
                "Completed",              // 6
                "Rated"                   // 7
            };

            // Step 8: Build the timeline stages list
            var stages = new List<ProgressStage>();

            for (int i = 1; i <= 7; i++)
            {
                StageState state;

                if (i < stage)
                    state = StageState.Completed;
                else if (i == stage)
                    state = StageState.Current;
                else
                    state = StageState.Pending;

                stages.Add(new ProgressStage
                {
                    Number = i,
                    Label = englishLabels[i],
                    LabelAr = arabicLabels[i],
                    Icon = GetIconForStage(i),
                    State = state
                });
            }

            // Step 9: Fill the ViewModel directly
            var vm = new RequestProgressViewModel();

            vm.Request = request;
            vm.ActiveAcceptance = acceptance;
            vm.ExistingRating = rating;
            vm.CurrentStage = stage;
            vm.CurrentStageLabel = englishLabels[stage];
            vm.CurrentStageArabic = arabicLabels[stage];
            vm.ProgressPercent = stage * 100 / 7;
            vm.IsRequester = isRequester;
            vm.IsVolunteer = isVolunteer;
            vm.ViewerUserId = userId.Value;
            vm.CanMarkComplete = isRequester && stage == 4;
            vm.CanRate = isRequester && stage == 6;
            vm.CanCancel = isRequester && stage <= 2;
            vm.Stages = stages;

            return View(vm);
        }

        // ─────────────────────────────────────────────────────────
        // PARTIAL VIEW: /RequestProgress/RefreshStatus/5
        // Returns only the alert card + progress bar HTML
        // Called by JavaScript every 30 seconds instead of JSON
        // ─────────────────────────────────────────────────────────
        public async Task<IActionResult> RefreshStatus(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var request = await _db.ServiceRequests
                .Include(r => r.Acceptances)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            Acceptance? acceptance = null;
            foreach (var a in request.Acceptances)
            {
                if (a.Status != AcceptanceStatus.Cancelled)
                {
                    acceptance = a;
                    break;
                }
            }

            Rating? rating = null;
            if (acceptance != null)
            {
                rating = await _db.Ratings
                    .FirstOrDefaultAsync(r => r.AcceptanceId == acceptance.AcceptanceId);
            }

            int stage = GetCurrentStage(request, acceptance, rating);

            // Pass the stage number and percent directly via ViewBag
            // No ViewModel, no JSON — just two numbers the partial view needs
            ViewBag.Stage = stage;
            ViewBag.Percent = stage * 100 / 7;

            string[] arabicLabels = {
                "", "تم إنشاء الطلب", "انتظار متطوع", "تم قبول المتطوع",
                "العمل جارٍ", "انتظار تأكيد الإتمام", "مكتمل", "تم التقييم"
            };

            ViewBag.StageLabel = arabicLabels[stage];

            return PartialView("_StatusCard");
        }

        // ─────────────────────────────────────────────────────────
        // HELPER: Decide which stage (1–7) the request is in
        // ─────────────────────────────────────────────────────────
        private int GetCurrentStage(ServiceRequest request, Acceptance? acceptance, Rating? rating)
        {
            if (rating != null)
                return 7; // Rated — fully done

            if (request.Status == RequestStatus.Completed)
                return 6; // Marked complete

            if (acceptance != null && acceptance.Status == AcceptanceStatus.Done)
                return 5; // Volunteer said "done", waiting for requester to confirm

            if (acceptance != null && acceptance.Status == AcceptanceStatus.InProgress)
                return 4; // Volunteer is working

            if (acceptance != null)
                return 3; // Volunteer accepted, not started yet

            if (request.Status == RequestStatus.Open)
                return 2; // Open, no volunteer yet

            return 1; // Just created
        }

        // ─────────────────────────────────────────────────────────
        // HELPER: Bootstrap icon name for each stage
        // ─────────────────────────────────────────────────────────
        private string GetIconForStage(int stageNumber)
        {
            if (stageNumber == 1) return "bi-plus-circle";
            if (stageNumber == 2) return "bi-hourglass-split";
            if (stageNumber == 3) return "bi-person-check";
            if (stageNumber == 4) return "bi-tools";
            if (stageNumber == 5) return "bi-clock-history";
            if (stageNumber == 6) return "bi-check-circle";
            if (stageNumber == 7) return "bi-star-fill";

            return "bi-circle";
        }
    }
}
