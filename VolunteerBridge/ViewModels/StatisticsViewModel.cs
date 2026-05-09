namespace VolunteerBridge.ViewModels
{
    public class StatisticsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalRequests { get; set; }
        public int TotalOpenRequests { get; set; }
        public int TotalCompletedRequests { get; set; }
        public int TotalVolunteers { get; set; }
        public int TotalPointsAwarded { get; set; }
        public string? TopCity { get; set; }
    }
}
