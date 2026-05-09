using VolunteerBridge.Models;

namespace VolunteerBridge.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int OpenRequests { get; set; }
        public int TotalPointsAwarded { get; set; }
        public List<User> RecentUsers { get; set; } = new();
        public List<ServiceRequest> RecentRequests { get; set; } = new();
    }
}
