# Common Issues & Debugging Guide — وصال (Wessal)

## Common Issues

---

### 🔴 Database Connection Fails on Startup

**Error:**
```
A network-related or instance-specific error occurred while establishing a connection to SQL Server
```

**Causes & Fixes:**

| Cause | Fix |
|-------|-----|
| LocalDB not running | Run: `sqllocaldb start MSSQLLocalDB` |
| Wrong instance name | Check `appsettings.json` — instance may be `MSSQLLocalDB` or `mssqllocaldb` |
| Migrations not applied | Run: `dotnet ef database update` |
| Database doesn't exist yet | Run: `dotnet ef database update` |

---

### 🔴 SignalR Chat Not Connecting

**Symptom:** Chat widget loads but messages don't send/receive. Browser console shows WebSocket errors.

**Fixes:**
1. Ensure you're on **HTTPS** — SignalR requires it in production
2. Check that `app.MapHub<ChatHub>("/hubs/chat")` is registered in `Program.cs` (before `MapControllerRoute`)
3. Ensure the SignalR JS client is loaded: Check `_Layout.cshtml` for `@microsoft/signalr` script
4. Session cookie must be present — if logged out, hub will reject the connection

---

### 🔴 Email Confirmation Fails / Emails Not Arriving

**Symptom:** Registration appears to succeed but no confirmation email arrives.

**Causes:**
1. **Empty SMTP password** — Check `appsettings.json` → `EmailSettings:Password`
2. **Brevo credentials invalid** — Log into Brevo dashboard, verify SMTP key is active
3. **Email goes to spam** — Check recipient's spam folder

**Quick Development Workaround:**
```sql
-- Manually confirm an email in the database (dev only)
UPDATE Users SET IsEmailConfirmed = 1 WHERE Email = 'your@email.com'
```

---

### 🔴 Chat Widget Not Appearing

**Symptom:** No floating chat button visible in the bottom-right corner.

**Cause:** The widget is only rendered for logged-in users. Check `_Layout.cshtml`:
```razor
@if (Context.Session.GetInt32("UserId") != null) {
    <partial name="_FloatingChat" />
}
```

**Fix:** Ensure you are logged in. If logged in and still not showing, check browser console for JS errors.

---

### 🔴 Session Expires Immediately / Unexpected Logouts

**Cause:** Session is stored in-memory (`AddDistributedMemoryCache`). An app restart clears all sessions.

**Fix for Development:** Increase timeout in `Program.cs`:
```csharp
options.IdleTimeout = TimeSpan.FromHours(8);
```

**Fix for Production:** Use Redis or SQL Server as session store.

---

### 🔴 Image Upload Fails

**Symptom:** Request is created but without an image, or an error about file types.

**Checks:**
1. Ensure `/wwwroot/uploads/` directory exists (it's created automatically on first upload)
2. Only allowed extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`
3. Check the `IWebHostEnvironment` is correctly injected in `ServiceRequestsController`

---

### 🔴 Admin Panel Redirects to Login (Despite Being Logged In as Regular User)

**Cause:** Admin session keys (`IsAdmin`, `AdminLoggedIn`) are separate from user session keys. Regular user login does not set admin keys.

**Fix:** Navigate to `/Admin/Login` and use admin credentials:
- Email: `admin@volunteerbridge.com`
- Password: `Admin123`

---

### 🔴 `dotnet ef` Commands Fail

**Error:** `Build failed` or `No DbContext was found`

**Fix:** Ensure you are in the project directory (not the solution root):
```bash
cd VolunteerBridge   # ← the project folder, not the solution root
dotnet ef migrations list
```

---

## Debugging Tips

### Check Session State
Add temporary debug output to any view:
```razor
@* Temporary debug — remove before commit *@
<p>UserId: @Context.Session.GetInt32("UserId")</p>
<p>UserName: @Context.Session.GetString("UserName")</p>
```

### Enable Detailed Errors in Development
`appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

Setting EF Core logging to `Information` logs all generated SQL queries — useful for debugging N+1 problems.

### Test SignalR Manually
Open browser DevTools → Network tab → Filter by "WS" (WebSocket). You should see a connection to `/hubs/chat`. Click it to inspect frames.

### Inspect Database Directly
Connect to LocalDB using **SQL Server Object Explorer** in Visual Studio:
- Server: `(localdb)\MSSQLLocalDB`
- Database: `VolunteerBridge`
