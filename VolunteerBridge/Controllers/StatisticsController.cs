using Microsoft.AspNetCore.Mvc;
using VolunteerBridge.Models;
using VolunteerBridge.ViewModels;

namespace VolunteerBridge.Controllers
{
    [Route("Admin/Statistics")]
    public class StatisticsController : Controller
    {
        private readonly AppDbContext _db;

        public StatisticsController(AppDbContext db)
        {
            _db = db;
        }

        [Route("")]
        public IActionResult Index()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            var adminLoggedIn = HttpContext.Session.GetString("AdminLoggedIn");

            if (isAdmin != "true" || adminLoggedIn != "true")
                return RedirectToAction("Login", "Admin");

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // 1. Deep Insights
            var mostActiveCategoryEnum = _db.ServiceRequests
                .Where(r => r.CreatedAt >= startOfMonth)
                .GroupBy(r => r.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => (Enums.RequestCategory?)g.Key)
                .FirstOrDefault();

            var mostActiveCategory = mostActiveCategoryEnum.HasValue 
                ? GetCategoryDisplayName(mostActiveCategoryEnum.Value) 
                : "لا توجد بيانات كافية";

            var peakDayEnum = _db.Acceptances
                .Select(a => a.AcceptedAt)
                .ToList()
                .GroupBy(d => d.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .Select(g => (DayOfWeek?)g.Key)
                .FirstOrDefault();

            var peakDay = peakDayEnum.HasValue ? GetArabicDay(peakDayEnum.Value) : "لا توجد بيانات";

            var totalReqs = _db.ServiceRequests.Count();
            var completedReqs = _db.ServiceRequests.Count(r => r.Status == Enums.RequestStatus.Completed);
            var completionRate = totalReqs > 0 ? ((double)completedReqs / totalReqs) * 100 : 0;

            var avgRating = _db.Ratings.Any() ? _db.Ratings.Average(r => (double)r.Rate) : 0;

            // 2. Monthly Trend (Last 6 Months)
            var trendMonths = new List<string>();
            var monthlyNewReqs = new List<int>();
            var monthlyCompleted = new List<int>();
            var monthlyNewUsers = new List<int>();

            var sixMonthsAgo = now.AddMonths(-5);
            var startDate = new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1);

            var requestsData = _db.ServiceRequests
                .Where(r => r.CreatedAt >= startDate)
                .ToList()
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:D2}", g => new {
                    Total = g.Count(),
                    Completed = g.Count(x => x.Status == Enums.RequestStatus.Completed)
                });

            var usersData = _db.Users
                .Where(u => u.CreatedAt >= startDate)
                .ToList()
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:D2}", g => g.Count());

            var arabicCulture = new System.Globalization.CultureInfo("ar-SA");

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                string key = $"{monthDate.Year}-{monthDate.Month:D2}";
                trendMonths.Add(monthDate.ToString("MMMM", arabicCulture));

                monthlyNewReqs.Add(requestsData.ContainsKey(key) ? requestsData[key].Total : 0);
                monthlyCompleted.Add(requestsData.ContainsKey(key) ? requestsData[key].Completed : 0);
                monthlyNewUsers.Add(usersData.ContainsKey(key) ? usersData[key] : 0);
            }

            // 3. Category Distribution
            var catData = _db.ServiceRequests
                .GroupBy(r => r.Category)
                .Select(g => new { Cat = g.Key, Count = g.Count() })
                .ToList();

            var reqsByCategory = new Dictionary<string, int>();
            foreach(var item in catData)
            {
                reqsByCategory[GetCategoryDisplayName(item.Cat)] = item.Count;
            }

            // 4. Top Cities
            var topCities = _db.Users
                .Where(u => !string.IsNullOrEmpty(u.City))
                .GroupBy(u => u.City)
                .Select(g => new { City = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToDictionary(x => x.City!, x => x.Count);

            // 5. Ratings Distribution
            var ratingsDist = _db.Ratings
                .GroupBy(r => r.Rate)
                .Select(g => new { Stars = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Stars, x => x.Count);
            
            for (int i=1; i<=5; i++) {
                if (!ratingsDist.ContainsKey(i)) ratingsDist[i] = 0;
            }

            // 6. Top Volunteers this month
            var topVols = _db.pointTransactions
                .Where(pt => pt.CreatedAt >= startOfMonth)
                .GroupBy(pt => pt.UserId)
                .Select(g => new {
                    UserId = g.Key,
                    MonthlyPoints = g.Sum(x => x.Points)
                })
                .OrderByDescending(x => x.MonthlyPoints)
                .Take(5)
                .ToList();

            var topVolunteersList = new List<TopVolunteerDto>();
            foreach(var tv in topVols)
            {
                var user = _db.Users.Find(tv.UserId);
                if (user != null)
                {
                    topVolunteersList.Add(new TopVolunteerDto {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Initials = string.IsNullOrWhiteSpace(user.FullName) ? "" : user.FullName.Substring(0, 1),
                        MonthlyPoints = tv.MonthlyPoints
                    });
                }
            }

            var vm = new StatisticsViewModel
            {
                MostActiveCategory = mostActiveCategory,
                PeakVolunteeringDay = peakDay,
                TaskCompletionRate = completionRate,
                AverageResponseRating = avgRating,
                TrendMonths = trendMonths,
                MonthlyNewRequests = monthlyNewReqs,
                MonthlyCompletedTasks = monthlyCompleted,
                MonthlyNewUsers = monthlyNewUsers,
                RequestsByCategory = reqsByCategory,
                TopCitiesActivity = topCities,
                RatingsDistribution = ratingsDist,
                MonthlyTopVolunteers = topVolunteersList
            };

            return View(vm);
        }

        private string GetCategoryDisplayName(Enums.RequestCategory cat)
        {
            return cat switch
            {
                Enums.RequestCategory.Community => "مجتمعي",
                Enums.RequestCategory.Education => "تعليمي",
                Enums.RequestCategory.Tech => "تقني",
                Enums.RequestCategory.Healthcare => "صحي",
                Enums.RequestCategory.Environment => "بيئي",
                Enums.RequestCategory.Events => "فعاليات",
                Enums.RequestCategory.Humanitarian => "إنساني",
                Enums.RequestCategory.Mentorship => "إرشاد",
                Enums.RequestCategory.AnimalWelfare => "رعاية الحيوانات",
                _ => "أخرى"
            };
        }

        private string GetArabicDay(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Saturday => "السبت",
                DayOfWeek.Sunday => "الأحد",
                DayOfWeek.Monday => "الإثنين",
                DayOfWeek.Tuesday => "الثلاثاء",
                DayOfWeek.Wednesday => "الأربعاء",
                DayOfWeek.Thursday => "الخميس",
                DayOfWeek.Friday => "الجمعة",
                _ => ""
            };
        }
    }
}
