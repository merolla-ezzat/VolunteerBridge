# Core Features — وصال (Wessal)

## 1. Authentication System

### Registration
- User fills in full name, email, phone, city, and password
- Password is hashed immediately using **BCrypt.Net** before being stored in session
- A confirmation token (GUID) is generated and emailed via **Brevo SMTP**
- User data is held in session (`PendingUser_*` keys) until email is confirmed
- On confirmation click, data is flushed from session and written to the database
- User is then redirected to complete their profile (bio, skills, experience)

### Login
- Email/password validation against BCrypt hash
- Banned users are blocked with a descriptive message
- Unconfirmed emails are blocked from logging in
- Successful login stores `UserId` and `UserName` in session

### Session-Based Authorization
- All protected routes use `HttpContext.Session.GetInt32("UserId")` as an auth gate
- No ASP.NET Core Identity — a fully custom implementation
- Admin access is gated on `Session["IsAdmin"] == "true"` AND `Session["AdminLoggedIn"] == "true"`

---

## 2. Service Request System

### Creating a Request
- Fields: Title, Category (10 options), Description, Location, Scheduled Date, Estimated Hours, Image
- Image upload: validated by extension (`.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`), stored in `/wwwroot/uploads/` with a GUID filename
- `PointsReward` is auto-calculated: `max(10, estimatedHours × 20)`
- Status defaults to `Open`

### Browsing & Filtering
- Public feed shows all `Open` requests that are not admin-removed
- Filters: free-text search (title/description), category dropdown, city text
- Results are ordered by `CreatedAt` descending

### Request Lifecycle
```
Open → Accepted → Completed → (Rated)
     ↘ Cancelled
```
- Only the **requester** can cancel an `Open` request
- Status moves to `Accepted` when a volunteer accepts
- Status moves to `Completed` when the requester marks the task done
- Admin can **soft-delete** (flag `IsRemovedByAdmin`) without losing data

---

## 3. Volunteer Acceptance System

- Volunteers browse open requests and click "Accept"
- The system prevents:
  - Self-acceptance (requester cannot accept their own request)
  - Duplicate acceptance by the same volunteer
- On acceptance: `Acceptance` record is created with `Status = InProgress`
- The `ServiceRequest.Status` is immediately updated to `Accepted`

### Task Completion
- Only the **requester** can mark a task complete (via `AcceptancesController.MarkComplete`)
- On completion:
  - `Acceptance.Status → Done`, `Acceptance.CompletedAt` recorded
  - `ServiceRequest.Status → Completed`
  - Volunteer earns `PointsReward` points
  - `PointTransaction` record is created for audit trail
  - Volunteer's level is recalculated based on total points:

| Points | Level |
|--------|-------|
| 0–99 | مبتدئ الخير (Newcomer) |
| 100–299 | صانع الفرق (Helper) |
| 300–699 | موثوق العطاء (Trusted) |
| 700+ | بطل المجتمع (Champion) |

---

## 4. Real-Time Chat System (SignalR)

- Every authenticated user gets a **floating chat widget** (rendered in `_Layout.cshtml` via `_FloatingChat.cshtml`)
- The widget manages two views: **Inbox** and **Thread**
- Chat uses **ASP.NET Core SignalR** at `/hubs/chat`
- Messages are persisted to the `ChatMessages` table
- Unread count is fetched on page load and updated in real-time
- The chat bubble glows when a new message arrives while the widget is closed

See [`/docs/chat-system/signalr-architecture.md`](../chat-system/signalr-architecture.md) for full flow.

---

## 5. Request Progress Tracker

A visual 7-stage timeline that both the requester and volunteer can view. The stages are:

| Stage | Arabic | Trigger |
|-------|--------|---------|
| 1 | تم إنشاء الطلب | Request created |
| 2 | انتظار متطوع | Request is Open |
| 3 | تم قبول المتطوع | Acceptance exists |
| 4 | العمل جارٍ | Acceptance is InProgress |
| 5 | انتظار تأكيد الإتمام | Acceptance is Done |
| 6 | مكتمل | ServiceRequest is Completed |
| 7 | تم التقييم | Rating exists |

- Progress percentage = `stage × 100 / 7`
- The page auto-refreshes via a polling partial view (`RefreshStatus`) every 30 seconds
- Only the requester and the assigned volunteer can view this page

---

## 6. Points & Leaderboard

- Points are accumulated per volunteer
- All point transactions are recorded in `PointTransactions` for a full audit trail
- The **Leaderboard** (`LeaderboardController`) shows a ranked list of users by `TotalPoints`
- The admin **Statistics** page shows top volunteers by points earned in the current month

---

## 7. Ratings System

- After a task is marked `Completed` (stage 6), the requester can rate the volunteer (1–5 stars + optional comment)
- Duplicate ratings are blocked at both the UI level (button disabled) and the controller level
- On submission, the volunteer's `AverageRating` is recalculated as the mean of all received ratings
- Rating moves the progress tracker to stage 7

---

## 8. Admin Panel

Accessible at `/Admin/Login` with credentials stored in `appsettings.json`:

| Capability | Description |
|------------|-------------|
| Dashboard | KPI cards (users, requests, completions, points) + activity feed |
| User Management | List all users with pagination (15/page), ban/unban with reason |
| Request Management | List all requests with pagination (15/page), soft-remove with reason |
| Statistics | Charts — monthly trends, category distribution, top cities, rating distribution |

> ⚠️ **Security Issue:** Admin credentials are stored in plaintext in `appsettings.json`. See deployment notes.

---

## 9. Email Confirmation

- Uses **MailKit** with **Brevo** (formerly Sendinblue) as the SMTP relay
- Confirmation email is HTML with a branded "Confirm Account" button
- Token is a GUID compared against the session-stored token
- If the SMTP send fails, registration is halted with a user-visible error

---

## 10. Profile System

- Users can view their own profile (`/Account/Profile`) and others' profiles (`/Account/ViewProfile/{id}`)
- Profile data: FullName, Email, City, PhoneNumber, Bio, Skills, Experience
- Level badge and AverageRating are computed fields displayed on the profile
- Edit profile is a separate form; no re-authentication required to change email (potential security issue)
