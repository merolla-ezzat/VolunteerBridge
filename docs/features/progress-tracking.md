# Progress Tracking System — Feature Documentation

## Overview

The Request Progress Tracker is a **7-stage visual timeline** that allows both the requester and the volunteer to see exactly where a service request stands in its lifecycle. Implemented in `RequestProgressController` with a dedicated CSS file (`progress-tracker.css`).

---

## The 7 Stages

| # | English Label | Arabic Label | Trigger Condition |
|---|---------------|--------------|-------------------|
| 1 | Request Created | تم إنشاء الطلب | Always first stage |
| 2 | Waiting for Volunteer | انتظار متطوع | `Status == Open` |
| 3 | Volunteer Accepted | تم قبول المتطوع | `Acceptance exists` (any non-cancelled) |
| 4 | Task In Progress | العمل جارٍ | `Acceptance.Status == InProgress` |
| 5 | Waiting for Completion | انتظار تأكيد الإتمام | `Acceptance.Status == Done` |
| 6 | Completed | مكتمل | `ServiceRequest.Status == Completed` |
| 7 | Rated | تم التقييم | `Rating exists` |

### Stage Determination Logic

```csharp
private int GetCurrentStage(ServiceRequest request, Acceptance? acceptance, Rating? rating)
{
    if (rating != null)                                         return 7;
    if (request.Status == RequestStatus.Completed)              return 6;
    if (acceptance?.Status == AcceptanceStatus.Done)            return 5;
    if (acceptance?.Status == AcceptanceStatus.InProgress)      return 4;
    if (acceptance != null)                                     return 3;
    if (request.Status == RequestStatus.Open)                   return 2;
    return 1;
}
```

Progress percentage = `stage × 100 / 7`

---

## Stage Visual States

Each stage is rendered with one of three visual states:

| State | CSS Class | Visual |
|-------|-----------|--------|
| `Completed` | completed | Green, checkmark icon |
| `Current` | current | Indigo, glowing animation |
| `Pending` | pending | Gray, faded |

---

## Access Control

Only **two users** can access the progress tracker page:

```csharp
bool isRequester = (request.RequesterId == userId);
bool isVolunteer = (acceptance != null && acceptance.VolunteerId == userId);

if (!isRequester && !isVolunteer)
    return Forbid();
```

---

## Action Flags

The ViewModel exposes action flags based on the viewer's role and current stage:

| Flag | Condition | Action Shown |
|------|-----------|-------------|
| `CanMarkComplete` | `isRequester && stage == 4` | "Mark as Complete" button |
| `CanRate` | `isRequester && stage == 6` | "Rate Volunteer" button |
| `CanCancel` | `isRequester && stage <= 2` | "Cancel Request" button |

---

## Auto-Refresh (Polling)

The progress page auto-refreshes every 30 seconds via a partial view endpoint:

```javascript
// In the Razor view (implied by the RefreshStatus endpoint)
setInterval(() => {
    fetch('/RequestProgress/RefreshStatus/' + requestId)
        .then(r => r.text())
        .then(html => { document.getElementById('status-card').innerHTML = html; });
}, 30000);
```

The `RefreshStatus` endpoint returns a rendered `_StatusCard` partial view (not JSON), passing only two values:
```csharp
ViewBag.Stage = stage;
ViewBag.Percent = stage * 100 / 7;
ViewBag.StageLabel = arabicLabels[stage];
return PartialView("_StatusCard");
```

This avoids the need to re-render the full page for minor status updates.

---

## Bootstrap Icons Used

| Stage | Icon |
|-------|------|
| 1 | `bi-plus-circle` |
| 2 | `bi-hourglass-split` |
| 3 | `bi-person-check` |
| 4 | `bi-tools` |
| 5 | `bi-clock-history` |
| 6 | `bi-check-circle` |
| 7 | `bi-star-fill` |

---

## ViewModel Structure

```csharp
public class RequestProgressViewModel
{
    // Core entities
    public ServiceRequest Request { get; set; }
    public Acceptance? ActiveAcceptance { get; set; }
    public Rating? ExistingRating { get; set; }

    // Stage info
    public int CurrentStage { get; set; }        // 1-7
    public string CurrentStageLabel { get; set; } // English label
    public string CurrentStageArabic { get; set; } // Arabic label
    public int ProgressPercent { get; set; }      // 0-100

    // Viewer context
    public bool IsRequester { get; set; }
    public bool IsVolunteer { get; set; }
    public int ViewerUserId { get; set; }

    // Action flags
    public bool CanMarkComplete { get; set; }
    public bool CanRate { get; set; }
    public bool CanCancel { get; set; }

    // Stage list for timeline rendering
    public List<ProgressStage> Stages { get; set; }
}
```

---

## Known Limitations

| Limitation | Impact |
|-----------|--------|
| Polling-based refresh (30s) instead of SignalR | 30-second lag in progress updates |
| Cancelled requests not shown in tracker | No visibility into why a request was cancelled |
| Stage 5 ("Waiting for completion") is not explicitly transitioned | Volunteer currently cannot independently set status to "Done" — requester marks complete directly |
