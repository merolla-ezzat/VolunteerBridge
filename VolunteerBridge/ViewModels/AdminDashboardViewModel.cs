using VolunteerBridge.Models;

using VolunteerBridge.Models;

namespace VolunteerBridge.ViewModels
{
    public class ActivityItem
    {
        public string UserInitials { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public double UsersGrowth { get; set; }

        public int TotalRequests { get; set; }
        
        public int CompletedRequests { get; set; }
        public double CompletedTasksGrowth { get; set; }
        
        public int OpenRequests { get; set; }
        public double ActiveRequestsGrowth { get; set; }
        
        public int ActiveVolunteers { get; set; }
        public int BannedUsers { get; set; }
        
        public int TotalPointsAwarded { get; set; }
        public double PointsGrowth { get; set; }

        public List<User> TopVolunteers { get; set; } = new();
        public List<ActivityItem> RecentActivities { get; set; } = new();

        public List<User> RecentUsers { get; set; } = new();
        public List<ServiceRequest> RecentRequests { get; set; } = new();
        public List<Acceptance> RecentCompletedTasks { get; set; } = new();
        public List<Rating> RecentRatings { get; set; } = new();
    }
}
