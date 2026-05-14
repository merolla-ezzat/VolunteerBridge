# Folder Structure — وصال (Wessal)

## Repository Root

```
VolunteerBridge/                   ← Solution root
├── VolunteerBridge.sln            ← Visual Studio solution file
├── README.md                      ← Project overview
├── docs/                          ← THIS documentation system
├── .git/                          ← Git version control
├── .gitignore
└── VolunteerBridge/               ← Main ASP.NET Core project
```

## Project Directory — `VolunteerBridge/`

```
VolunteerBridge/
│
├── Controllers/                   ← 9 MVC Controllers
│   ├── AccountController.cs       ← Auth: register, login, logout, profile
│   ├── AcceptancesController.cs   ← Volunteer acceptance, task completion, ratings
│   ├── AdminController.cs         ← Admin dashboard, user/request management
│   ├── ChatController.cs          ← Chat inbox, message history, unread count APIs
│   ├── HomeController.cs          ← Landing page (Index)
│   ├── LeaderboardController.cs   ← Top volunteers ranked by points
│   ├── PointsController.cs        ← User point history view
│   ├── RequestProgressController.cs ← 7-stage progress tracker
│   └── ServiceRequestsController.cs ← CRUD for service requests
│
├── Helpers/
│   └── EnumExtensions.cs          ← GetDisplayName() extension for enum display in Arabic
│
├── Hubs/
│   └── ChatHub.cs                 ← SignalR Hub: SendMessage, JoinThread, OnConnectedAsync
│
├── Migrations/                    ← EF Core auto-generated migrations (7 total)
│   ├── 20260430154446_init.cs
│   ├── 20260502223559_AddEmailConfirmation.cs
│   ├── 20260508095519_AddImagePathToServiceRequest.cs
│   ├── 20260510010951_AddBanToUser.cs
│   ├── 20260510012307_AddRequestRemoval.cs
│   ├── 20260510222839_AddBioSkillsExperience.cs
│   ├── 20260513195000_AddChatMessages.cs
│   └── AppDbContextModelSnapshot.cs
│
├── Models/                        ← Domain entities + EF DbContext
│   ├── AppDbContext.cs            ← DbContext with all DbSets and fluent configuration
│   ├── Acceptance.cs              ← Volunteer acceptance record
│   ├── Enums.cs                   ← UserLevel, RequestStatus, AcceptanceStatus, RequestCategory
│   ├── ErrorViewModel.cs          ← Default error view model
│   ├── PointTransaction.cs        ← Points audit log
│   ├── Rating.cs                  ← Volunteer rating (1-5 stars)
│   ├── ServiceRequest.cs          ← Volunteer request entity
│   └── User.cs                    ← Platform user entity
│
├── Services/
│   └── EmailService.cs            ← MailKit SMTP via Brevo (email confirmation)
│
├── ViewModels/                    ← View-specific DTOs
│   ├── AdminDashboardViewModel.cs ← Dashboard KPIs + activity feed
│   ├── AdminLoginViewModel.cs     ← Admin login form
│   ├── BrowseFilterViewModel.cs   ← Browse page filters + results
│   ├── ChatInboxViewModel.cs      ← Inbox rows + active thread state
│   ├── ChatThreadViewModel.cs     ← Thread metadata
│   ├── CreateRequestViewModel.cs  ← Request creation form + image upload
│   ├── LoginViewModel.cs          ← User login form
│   ├── RatingViewModel.cs         ← Rating form
│   ├── RegisterViewModel.cs       ← User registration form
│   ├── RequestProgressViewModel.cs ← 7-stage progress tracker state
│   └── StatisticsViewModel.cs     ← Admin statistics charts data
│
├── Views/                         ← Razor views organized by controller
│   ├── Acceptances/               ← MyTasks.cshtml, Rate.cshtml
│   ├── Account/                   ← Login, Register, Profile, EditProfile,
│   │                                 ViewProfile, CheckEmail, ConfirmEmail,
│   │                                 CompleteProfile views
│   ├── Admin/                     ← Dashboard, Login, Users, Requests views
│   ├── Chat/                      ← Inbox.cshtml
│   ├── Home/                      ← Index.cshtml
│   ├── Leaderboard/               ← Index.cshtml
│   ├── Points/                    ← History.cshtml
│   ├── RequestProgress/           ← Index.cshtml, _StatusCard.cshtml (partial)
│   ├── ServiceRequests/           ← Browse, MyRequests, Create, Details views
│   ├── Shared/
│   │   ├── _Layout.cshtml         ← Main layout (navbar, floating chat, scripts)
│   │   ├── _AdminLayout.cshtml    ← Admin-specific layout (sidebar nav)
│   │   ├── _FloatingChat.cshtml   ← Chat widget HTML partial
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Statistics/                ← Index.cshtml (charts with Chart.js)
│   ├── _ViewImports.cshtml        ← Global using directives
│   └── _ViewStart.cshtml          ← Default layout binding
│
├── wwwroot/                       ← Static assets served by UseStaticFiles
│   ├── css/
│   │   ├── wessal.css             ← Main design system (1174 lines, ~34KB)
│   │   ├── wessal-chat.css        ← Floating chat widget styles (12KB)
│   │   ├── admin.css              ← Admin panel styles (18KB)
│   │   ├── progress-tracker.css   ← Progress timeline styles (4KB)
│   │   └── site.css               ← Legacy minimal styles (~2KB)
│   ├── js/
│   │   ├── wessal-chat.js         ← Chat widget logic + SignalR client (485 lines)
│   │   └── site.js                ← Minimal legacy JS (231 bytes)
│   ├── images/                    ← Static brand assets
│   ├── uploads/                   ← User-uploaded images (runtime-created)
│   └── lib/                       ← Bootstrap, jQuery, validation (CDN alternatives)
│
├── Properties/
│   └── launchSettings.json        ← Dev server port config
│
├── Program.cs                     ← Application entry point + DI + middleware
├── appsettings.json               ← Connection string, email settings, admin credentials
├── appsettings.Development.json   ← Development-specific overrides
└── VolunteerBridge.csproj         ← Project file + NuGet package references
```

## Key Patterns

### Controller Naming
All controllers follow the `{Feature}Controller` convention. Auth checks are done inline at the top of each action using the `GetUserId()` helper pattern:

```csharp
private int? GetUserId() => HttpContext.Session.GetInt32("UserId");

public IActionResult MyAction()
{
    var userId = GetUserId();
    if (userId == null) return RedirectToAction("Login", "Account");
    // ...
}
```

### ViewModel Purpose
ViewModels are used exclusively for **form binding and view presentation**. Domain models (`User`, `ServiceRequest`, etc.) are passed directly to views when only read operations are needed (e.g., `Profile` view receives a `User` entity directly).

### CSS Architecture
No CSS-in-JS or utility-first framework. Everything uses the custom `wessal.css` design system with `w-` prefixed utility classes. Bootstrap 5 is included primarily for its grid system and a few components, but is mostly superseded by the Wessal system.
