using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;
using static VolunteerBridge.Models.Enums;

namespace VolunteerBridge.Controllers
{
    public class AcceptancesController : Controller
    {
        private readonly AppDbContext _db;

        public AcceptancesController(AppDbContext context)
        {
            _db = context;
        }

        private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Accept(int requestId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var request = _db.ServiceRequests.Find(requestId);
            if (request == null || request.Status != Enums.RequestStatus.Open)
            {
                return BadRequest("الطلب غير متاح");
            }

            // Modified by Yousef: prevent unsafe acceptance cases before creating the volunteer record.
            if (request.RequesterId == userId.Value)
            {
                TempData["Error"] = "لا يمكنك قبول طلبك الخاص.";
                return RedirectToAction("Details", "ServiceRequests", new { id = requestId });
            }

            bool alreadyAccepted = _db.Acceptances
                .Any(a => a.RequestId == requestId && a.VolunteerId == userId.Value);
            if (alreadyAccepted)
            {
                TempData["Error"] = "لقد قبلت هذا الطلب بالفعل.";
                return RedirectToAction("Details", "ServiceRequests", new { id = requestId });
            }

            var acceptance = new Acceptance()
            {
                RequestId = requestId,
                VolunteerId = userId.Value,
                Status = Enums.AcceptanceStatus.InProgress,
                Request = request,
                Volunteer = _db.Users.Find(userId.Value)
            };
            _db.Acceptances.Add(acceptance);
            // Modified by Yousef: keep the request status synchronized after a successful acceptance.
            request.Status = Enums.RequestStatus.Accepted;
            _db.SaveChanges();
            TempData["Success"] = "تم قبول الطلب بنجاح.";
            return RedirectToAction("MyTasks");
        }

        public IActionResult MyTasks()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Get my tasks and order them by date
            var tasks = _db.Acceptances
                .Include(a => a.Request)
                .Where(a => a.VolunteerId == userId)
                .OrderByDescending(a => a.AcceptedAt)
                .ToList();
            return View(tasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkComplete(int acceptanceId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var acceptance = _db.Acceptances
                .Include(a => a.Request)
                .FirstOrDefault(a => a.AcceptanceId == acceptanceId);
            if (acceptance == null)
            {
                return NotFound();
            }
            // Only the Requester can mark complete
            if (acceptance.Request?.RequesterId != userId)
            {
                return Forbid();
            }
            // Update statuses
            acceptance.Status = AcceptanceStatus.Done;
            acceptance.CompletedAt = DateTime.Now;
            acceptance.Request.Status = RequestStatus.Completed;

            // Give points to volunteer
            var volunteer = _db.Users.Find(acceptance.VolunteerId);
            if (volunteer != null)
            {
                var points = acceptance.Request.PointsReward;
                volunteer.TotalPoints += points;
                volunteer.CompletedTasksCount += 1;
                // Recalculate level based on new total points
                volunteer.Level = volunteer.TotalPoints switch
                {
                    < 100 => UserLevel.Newcomer,
                    < 300 => UserLevel.Helper,
                    < 700 => UserLevel.Trusted,
                    _ => UserLevel.Champion
                };

                // Save point transaction record
                _db.pointTransactions.Add(new PointTransaction
                {
                    UserId = volunteer.UserId,
                    Points = points,
                    Reason = $"اكتمل: {acceptance.Request.Title}",
                    AcceptanceId = acceptanceId,
                    User = volunteer
                });
            }
            _db.SaveChanges();
            TempData["Success"] = "تم تسجيل إنجاز المهمة وإضافة النقاط للمتطوع.";
            return RedirectToAction("Details", "ServiceRequests",
            new { id = acceptance.RequestId });
        }

        public IActionResult Rate(int acceptanceId, int toUserId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Modified by Yousef: only allow the requester of the completed task to open the rating form.
            var acceptance = _db.Acceptances
                .Include(a => a.Request)
                .FirstOrDefault(a => a.AcceptanceId == acceptanceId);

            if (acceptance == null)
            {
                return NotFound();
            }

            if (acceptance.Request?.RequesterId != userId.Value ||
                acceptance.VolunteerId != toUserId ||
                acceptance.Status != AcceptanceStatus.Done)
            {
                return Forbid();
            }

            return View(new RatingViewModel
            {
                AcceptanceId = acceptanceId,
                ToUserId = toUserId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Rate(RatingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var acceptance = _db.Acceptances
                .Include(a => a.Request)
                .FirstOrDefault(a => a.AcceptanceId == model.AcceptanceId);

            if (acceptance == null)
            {
                return NotFound();
            }
            //Only requester can rate the Volunteer
            if (acceptance.Request?.RequesterId != userId)
            {
                return Forbid();
            }
            model.ToUserId = acceptance.VolunteerId;
            // Prevent duplicate ratings
            //حماية اضافية عشان لو حد بعت post request مباشرة من غير ال view
            //هو كدا كدا مش هيوصل عشان بعمل disable button rating from Details View
            bool alreadyRated = _db.Ratings.Any
                (r => r.AcceptanceId == model.AcceptanceId && r.FromUserId == userId);
            if (alreadyRated)
            {
                ModelState.AddModelError("", "لقد قمت بتقييم هذا المتطوع من قبل");
                return View(model);
            }
            _db.Ratings.Add(new Rating
            {
                AcceptanceId = model.AcceptanceId,
                FromUserId = userId.Value,
                ToUserId = model.ToUserId,
                Rate = model.Rate,
                Comment = model.Comment,
                Acceptance = _db.Acceptances.Find(model.AcceptanceId)!,
                FromUser = _db.Users.Find(userId.Value)!,
                ToUser = _db.Users.Find(model.ToUserId)!
            });

            _db.SaveChanges();
            var avg = _db.Ratings
                .Where(r => r.ToUserId == model.ToUserId)
                .Average(r => r.Rate);
            var ratedUser = _db.Users.Find(model.ToUserId);
            if (ratedUser != null)
            {
                ratedUser.AverageRating = (decimal)avg;
                _db.SaveChanges();
            }
            TempData["Success"] = "تم إرسال التقييم بنجاح.";
            return RedirectToAction("Details", "ServiceRequests", new { id = acceptance.RequestId });
        }
    }
}
