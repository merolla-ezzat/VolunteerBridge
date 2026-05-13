using System.Collections.Generic;

namespace VolunteerBridge.ViewModels
{
    public class StatisticsViewModel
    {
        // Deep Insights (Top Cards)
        public string MostActiveCategory { get; set; } = string.Empty;
        public string PeakVolunteeringDay { get; set; } = string.Empty;
        public double TaskCompletionRate { get; set; }
        public double AverageResponseRating { get; set; }

        // Line Chart Data (Monthly Trend)
        public List<string> TrendMonths { get; set; } = new List<string>();
        public List<int> MonthlyNewRequests { get; set; } = new List<int>();
        public List<int> MonthlyCompletedTasks { get; set; } = new List<int>();
        public List<int> MonthlyNewUsers { get; set; } = new List<int>();

        // Donut Chart Data (Categories)
        public Dictionary<string, int> RequestsByCategory { get; set; } = new Dictionary<string, int>();

        // Bar Chart Data (Top Cities)
        public Dictionary<string, int> TopCitiesActivity { get; set; } = new Dictionary<string, int>();

        // Bar Chart Data (Ratings)
        public Dictionary<int, int> RatingsDistribution { get; set; } = new Dictionary<int, int>();

        // Leaderboard
        public List<TopVolunteerDto> MonthlyTopVolunteers { get; set; } = new List<TopVolunteerDto>();
    }

    public class TopVolunteerDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public int MonthlyPoints { get; set; }
    }
}
