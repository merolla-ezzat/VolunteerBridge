// ViewModels/RequestProgressViewModel.cs
using VolunteerBridge.Models;

namespace VolunteerBridge.ViewModels
{
    //merolla+menna
    public class RequestProgressViewModel
    {
        // ── Core entities ──────────────────────────────────────
        public ServiceRequest Request { get; set; } = null!;
        public Acceptance? ActiveAcceptance { get; set; }
        public Rating? ExistingRating { get; set; }

        // ── Computed stage (1–7) ───────────────────────────────
        public int CurrentStage { get; set; }
        public string CurrentStageLabel { get; set; } = string.Empty;
        public string CurrentStageArabic { get; set; } = string.Empty;
        public int ProgressPercent { get; set; }

        // ── Viewer context ────────────────────────────────────
        public bool IsRequester { get; set; }
        public bool IsVolunteer { get; set; }
        public int ViewerUserId { get; set; }

        // ── Action flags ─────────────────────────────────────
        public bool CanMarkComplete { get; set; }
        public bool CanRate { get; set; }
        public bool CanCancel { get; set; }

        // ── Stage definitions (for rendering the timeline) ───
        public List<ProgressStage> Stages { get; set; } = new();
    }

    public class ProgressStage
    {
        public int Number { get; set; }
        public string Label { get; set; } = string.Empty;
        public string LabelAr { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;  // Bootstrap icon name
        public StageState State { get; set; }
    }

    public enum StageState
    {
        Completed,   // green, checkmark
        Current,     // blue, animated glow
        Pending      // gray, faded
    }
}
