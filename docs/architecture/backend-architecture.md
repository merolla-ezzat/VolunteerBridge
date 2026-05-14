# Backend Architecture — وصال (Wessal)

## Controller Overview

| Controller | Route Pattern | Key Responsibilities |
|-----------|--------------|---------------------|
| `AccountController` | `/Account/*` | Auth, profile, email confirmation |
| `ServiceRequestsController` | `/ServiceRequests/*` | CRUD for requests, image upload |
| `AcceptancesController` | `/Acceptances/*` | Volunteer lifecycle, completion, rating |
| `ChatController` | `/Chat/*` | Chat REST APIs (inbox, history, unread) |
| `AdminController` | `/Admin/*` | Admin login, dashboard, user/request management |
| `RequestProgressController` | `/RequestProgress/*` | 7-stage tracker page + partial refresh |
| `StatisticsController` | `/Admin/Statistics/*` | Analytics charts data |
| `LeaderboardController` | `/Leaderboard/*` | Ranked volunteer list |
| `PointsController` | `/Points/*` | Point transaction history |

## Shared Controller Patterns

### Auth Guard (repeated in every controller)
```csharp
private int? GetUserId() => HttpContext.Session.GetInt32("UserId");
```
This pattern is **duplicated across 7 controllers**. It should be extracted into a base controller class.

### TempData for Flash Messages
All success/error notifications use `TempData`:
```csharp
TempData["Success"] = "تم نشر طلبك بنجاح!";
TempData["Error"] = "لا يمكنك قبول طلبك الخاص.";
```
Rendered in `_Layout.cshtml` at the top of each page.

## Anti-Forgery Tokens

All `[HttpPost]` mutations use `[ValidateAntiForgeryToken]` — correctly protecting against CSRF.

## EnumExtensions Helper

```csharp
// Helpers/EnumExtensions.cs
public static string GetDisplayName(this Enum value) { ... }
```

Used in views to render the Arabic display names from enum values (e.g., `RequestCategory.Education` → "تعليمي").
