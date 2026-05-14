# Environment Variables & Configuration — وصال (Wessal)

## Configuration Sources

ASP.NET Core uses a layered configuration system. For Wessal, the priority (highest to lowest) is:

1. **Environment Variables** (production)
2. **`appsettings.Development.json`** (development overrides)
3. **`appsettings.json`** (base configuration)

---

## `appsettings.json` Reference

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VolunteerBridge;..."
  },
  "EmailSettings": {
    "Host": "smtp-relay.brevo.com",
    "Port": 587,
    "SenderName": "وصال",
    "SenderEmail": "sayedmayar308@gmail.com",
    "Username": "aa0dec001@smtp-brevo.com",
    "Password": ""
  },
  "AdminSettings": {
    "Email": "admin@volunteerbridge.com",
    "Password": "Admin123"
  }
}
```

---

## Configuration Keys Reference

| Key | Type | Description | Required |
|-----|------|-------------|----------|
| `ConnectionStrings:DefaultConnection` | string | SQL Server connection string | ✅ |
| `EmailSettings:Host` | string | SMTP server hostname | ✅ |
| `EmailSettings:Port` | int | SMTP port (587 for STARTTLS) | ✅ |
| `EmailSettings:SenderName` | string | Display name in "From" field | ✅ |
| `EmailSettings:SenderEmail` | string | From email address | ✅ |
| `EmailSettings:Username` | string | SMTP authentication username | ✅ |
| `EmailSettings:Password` | string | SMTP authentication password | ✅ |
| `AdminSettings:Email` | string | Admin panel login email | ✅ |
| `AdminSettings:Password` | string | Admin panel login password | ✅ |

---

## Production Environment Variables

For production, **never store secrets in `appsettings.json`**. Use environment variables instead:

```bash
# Set via system environment, Docker ENV, or Azure App Service application settings

ConnectionStrings__DefaultConnection="Server=your-server;Database=VolunteerBridge;..."
EmailSettings__Password="your-brevo-api-key"
AdminSettings__Password="YourSecureAdminPassword"
```

ASP.NET Core automatically maps `__` (double underscore) to `:` in configuration keys.

---

## Azure App Service Setup

If deploying to Azure App Service, add these Application Settings in the Azure Portal:

| Setting Name | Value |
|-------------|-------|
| `ConnectionStrings__DefaultConnection` | Azure SQL connection string |
| `EmailSettings__Password` | Brevo API key |
| `AdminSettings__Email` | your-admin@domain.com |
| `AdminSettings__Password` | Strong password |

---

## Session Configuration

Configured in `Program.cs` — not in `appsettings.json`:

```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // 1 hour idle timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

To change session timeout for production, modify `Program.cs` or expose it as a configuration key.

---

## ⚠️ Security Checklist Before Deployment

- [ ] Change `AdminSettings:Password` from `Admin123` to a strong password
- [ ] Store `EmailSettings:Password` as an environment variable, not in `appsettings.json`
- [ ] Ensure `appsettings.json` is in `.gitignore` or strip credentials before committing
- [ ] Set `AdminSettings:Email` to a real admin email address
- [ ] Verify `AllowedHosts` is restricted (not `"*"`) for production
- [ ] Use Azure SQL or a managed database instead of LocalDB
