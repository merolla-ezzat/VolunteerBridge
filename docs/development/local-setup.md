# Local Development Setup — وصال (Wessal)

## Prerequisites

| Tool | Version | Download |
|------|---------|---------|
| .NET SDK | 9.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| SQL Server LocalDB | 2019+ | Included with Visual Studio, or [SQL Server Express](https://www.microsoft.com/sql-server) |
| Visual Studio | 2022 (recommended) | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |
| VS Code (alternative) | Latest | With C# Dev Kit extension |
| Git | Any | [git-scm.com](https://git-scm.com/) |

> **Note:** SQL Server LocalDB is automatically installed with Visual Studio. If using VS Code only, install SQL Server Express separately.

---

## Step-by-Step Setup

### 1. Clone the Repository

```bash
git clone https://github.com/merolla-ezzat/VolunteerBridge.git
cd VolunteerBridge
```

### 2. Switch to the Development Branch

```bash
# Use the latest integrated branch
git checkout Beta-Final
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Verify the Connection String

Open `VolunteerBridge/appsettings.json` and confirm the connection string:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VolunteerBridge;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

The LocalDB instance `(localdb)\MSSQLLocalDB` is the default. If you have a named SQL Server instance, replace it accordingly.

### 5. Apply Database Migrations

```bash
cd VolunteerBridge
dotnet ef database update
```

This will create the `VolunteerBridge` database and apply all 7 migrations. You should see output ending with:
```
Done.
```

### 6. Configure Email (Optional)

By default, email sending is configured for Brevo SMTP. If you don't have credentials, email confirmation will fail on registration.

**Option A:** Add your Brevo SMTP credentials to `appsettings.Development.json`:

```json
{
    "EmailSettings": {
        "Password": "your-brevo-api-key"
    }
}
```

**Option B:** Temporarily bypass email confirmation by manually setting `IsEmailConfirmed = true` in the database after registration.

### 7. Run the Application

```bash
dotnet run
```

Or in Visual Studio: Press **F5** (with debugging) or **Ctrl+F5** (without).

The app will be available at:
- `https://localhost:7xxx` (HTTPS — check terminal output for port)
- `http://localhost:5xxx` (HTTP redirect)

---

## Admin Access

The admin panel is at `/Admin` (or `/Admin/Login`).

Default credentials from `appsettings.json`:
```
Email:    admin@volunteerbridge.com
Password: Admin123
```

> ⚠️ Change these before any public deployment.

---

## First-Time Usage

1. Navigate to `/Account/Register`
2. Create an account (email confirmation required)
3. Browse requests at `/ServiceRequests/Browse`
4. Create a request at `/ServiceRequests/Create`
5. Chat with users via the floating chat button (bottom-right corner)

---

## Useful dotnet Commands

```bash
# List all migrations
dotnet ef migrations list

# Add a new migration after model changes
dotnet ef migrations add YourMigrationName

# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Drop and recreate database
dotnet ef database drop
dotnet ef database update

# View EF tool version
dotnet ef --version
```

---

## Troubleshooting

### "Cannot open database" error
- Ensure SQL Server LocalDB is running: `sqllocaldb start MSSQLLocalDB`
- Check the connection string matches your SQL Server instance name

### Chat not connecting
- Ensure you're accessing via HTTPS (SignalR WebSocket requires correct protocol)
- Check browser console for SignalR connection errors

### Email confirmation fails
- Check Brevo SMTP credentials in `appsettings.json`
- Temporarily skip by manually updating `IsEmailConfirmed = 1` in the Users table

### "Could not find migrations assembly" error
- Ensure you're running `dotnet ef` from the `VolunteerBridge/` project directory (not the solution root)

### Hot reload not working
- Run with `dotnet watch run` for Razor view hot-reload
