using VolunteerBridge.Models;

namespace VolunteerBridge.ViewModels
{
    public class BrowseFilterViewModel
    {
        public List<ServiceRequest> Requests { get; set; } = new();
        public string? SearchTerm { get; set; }
        public Enums.RequestCategory? Category { get; set; }
        public string? City { get; set; }
    }
}
