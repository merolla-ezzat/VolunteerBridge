# Deployment Guide — وصال (Wessal)

## Overview

This guide covers deploying Wessal to **Azure App Service** with **Azure SQL Database** — the recommended production stack. The same principles apply to any Windows-based hosting.

---

## Pre-Deployment Checklist

### Security
- [ ] Change `AdminSettings:Password` from `Admin123`
- [ ] Remove SMTP password from `appsettings.json` (use environment variables)
- [ ] Set `AllowedHosts` to your domain in `appsettings.json`
- [ ] Ensure HTTPS redirect is enabled (`app.UseHttpsRedirection()` — already in place)
- [ ] Verify `Cookie.HttpOnly = true` on session (already in place)

### Database
- [ ] Provision Azure SQL Database
- [ ] Run `dotnet ef database update` against production database
- [ ] Verify connection string works from the hosted environment

### Email
- [ ] Confirm Brevo SMTP credentials are valid
- [ ] Test email sending from production domain (whitelist sender domain in Brevo)

---

## Azure App Service Deployment

### Step 1: Create Azure Resources

```bash
# Install Azure CLI if needed
az login

# Create resource group
az group create --name WessalRG --location eastus

# Create App Service Plan (Linux or Windows)
az appservice plan create --name WessalPlan --resource-group WessalRG --sku B1

# Create Web App
az webapp create --name wessal-app --resource-group WessalRG --plan WessalPlan --runtime "DOTNET|9.0"

# Create Azure SQL Server
az sql server create --name wessal-sql --resource-group WessalRG --admin-user sqladmin --admin-password "YourStrongPassword123!"

# Create database
az sql db create --name VolunteerBridge --server wessal-sql --resource-group WessalRG --service-objective S0
```

### Step 2: Configure Application Settings

In the Azure Portal → App Service → Configuration → Application settings:

| Name | Value |
|------|-------|
| `ConnectionStrings__DefaultConnection` | `Server=wessal-sql.database.windows.net;Database=VolunteerBridge;User Id=sqladmin;Password=...;TrustServerCertificate=True` |
| `EmailSettings__Password` | `your-brevo-smtp-password` |
| `AdminSettings__Email` | `admin@yourdomain.com` |
| `AdminSettings__Password` | `YourSecureAdminPassword` |

### Step 3: Build and Deploy

```bash
# Publish the app
cd VolunteerBridge
dotnet publish -c Release -o ./publish

# Deploy to Azure (or use GitHub Actions — see Step 4)
az webapp deploy --resource-group WessalRG --name wessal-app --src-path ./publish
```

### Step 4: Apply Migrations Against Production DB

```bash
# Update the connection string in appsettings.json temporarily OR use:
dotnet ef database update --connection "your-azure-sql-connection-string"
```

---

## GitHub Actions CI/CD (Recommended)

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ Beta-Final ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore -c Release
    
    - name: Publish
      run: dotnet publish VolunteerBridge/VolunteerBridge.csproj -c Release -o ./publish
    
    - name: Deploy to Azure Web Apps
      uses: azure/webapps-deploy@v3
      with:
        app-name: 'wessal-app'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: './publish'
```

Store the Azure publish profile as a GitHub secret named `AZURE_WEBAPP_PUBLISH_PROFILE`.

---

## SignalR in Production

SignalR works out-of-the-box on Azure App Service with WebSockets. Ensure:

1. WebSockets are **enabled** in App Service → Configuration → General settings
2. If using a load balancer or multiple instances, add a **Redis backplane**:

```csharp
// Program.cs — add Redis backplane for multi-instance SignalR
builder.Services.AddSignalR().AddStackExchangeRedis("redis-connection-string");
```

---

## Upload Directory in Production

The `/wwwroot/uploads/` directory is **local to the App Service instance**. Images uploaded in production will be lost on app restart or scale-out.

**Recommended fix:** Replace local upload storage with **Azure Blob Storage**:

1. Install `Azure.Storage.Blobs` NuGet package
2. Create a Blob Storage container named `uploads`
3. Modify `ServiceRequestsController.Create` to upload to Blob instead of local filesystem
4. Store the blob URL in `ServiceRequest.ImagePath`

---

## Production Optimization Notes

| Area | Current State | Production Recommendation |
|------|--------------|--------------------------|
| Session | In-memory (lost on restart) | Use Azure Redis Cache |
| Image storage | Local filesystem | Use Azure Blob Storage |
| SignalR | Single-server only | Add Redis backplane |
| Logging | Console only | Add Application Insights |
| Secrets | `appsettings.json` | Use Azure Key Vault |
| CSS/JS | Unminified | Add bundling & minification |

---

## Health Check Endpoint

No health check is currently implemented. For production, add:

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

app.MapHealthChecks("/health");
```

This allows Azure to monitor application health and restart unhealthy instances automatically.
