# وصال (Wessal) — Volunteer Bridge Platform

> **Connecting people who need help with volunteers willing to give it.**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue)](https://docs.microsoft.com/en-us/aspnet/core/mvc)
[![SignalR](https://img.shields.io/badge/SignalR-Real--time%20Chat-red)](https://docs.microsoft.com/en-us/aspnet/core/signalr)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB%20%7C%20Azure-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-Academic%20Project-green)]()

---

## 📖 Description

**Wessal (وصال)** is an Arabic-first, RTL social volunteering platform built with ASP.NET Core MVC. It bridges the gap between people who need help and community volunteers willing to provide it. The platform features real-time chat, a gamified points system, a 7-stage request progress tracker, and a polished design system — all delivered in Arabic.

---

## 🖼️ Screenshots

> _Screenshots to be added here — see `/docs/overview/core-features.md` for feature descriptions._

| Home Page | Browse Requests | Chat Widget | Progress Tracker |
|-----------|-----------------|-------------|------------------|
| _coming soon_ | _coming soon_ | _coming soon_ | _coming soon_ |

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🔐 **Authentication** | Registration with email confirmation, BCrypt-hashed passwords, session-based auth |
| 📋 **Service Requests** | Create, browse, filter, and manage volunteer requests by category & city |
| 🤝 **Volunteer Acceptance** | Volunteers can accept open requests and mark tasks complete |
| 💬 **Real-time Chat** | SignalR-powered floating chat widget with inbox and message threads |
| 📊 **Progress Tracking** | 7-stage visual timeline tracking every request from creation to rating |
| 🏆 **Points & Leaderboard** | Gamified point rewards for completed tasks with 4 user levels |
| ⭐ **Ratings System** | Requesters rate volunteers after task completion |
| 🛡️ **Admin Panel** | Separate admin dashboard for user management, request moderation & analytics |
| 📈 **Statistics** | Charts and insights on platform activity, categories, and top volunteers |
| 🌐 **RTL-First Design** | Arabic-first interface with IBM Plex Sans Arabic + Rubik fonts |

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Framework** | ASP.NET Core MVC 9.0 |
| **Language** | C# |
| **Database** | SQL Server (LocalDB for dev, Azure SQL for prod) |
| **ORM** | Entity Framework Core 9 |
| **Real-time** | ASP.NET Core SignalR |
| **Authentication** | Custom session-based + BCrypt.Net-Next |
| **Email** | MailKit + Brevo SMTP relay |
| **Frontend** | Razor Views, Vanilla JS, Bootstrap 5 (utility layer) |
| **Design System** | Wessal CSS (`wessal.css`) — custom RTL-first design system |
| **Icons** | Bootstrap Icons + Google Material Icons |
| **Fonts** | Rubik (display), IBM Plex Sans Arabic (body) |

---

## 🏗️ Architecture Overview

```
Browser (Razor + Wessal CSS + wessal-chat.js)
       ↕ HTTP / SignalR WebSocket
ASP.NET Core MVC (Controllers)
       ↕ EF Core
SQL Server (AppDbContext)
       ↕ SMTP
Brevo Email Service
```

See [`/docs/architecture/system-architecture.md`](./docs/architecture/system-architecture.md) for full diagrams.

---

## 🚀 Quick Start

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/merolla-ezzat/VolunteerBridge.git
cd VolunteerBridge

# 2. Switch to the main integration branch
git checkout Beta-Final

# 3. Restore dependencies
dotnet restore

# 4. Apply database migrations
cd VolunteerBridge
dotnet ef database update

# 5. Configure secrets (see /docs/development/environment-variables.md)
# Update appsettings.Development.json with your SMTP credentials

# 6. Run the application
dotnet run
```

The app will be available at `https://localhost:7xxx` (port shown in terminal).

---

## 📁 Folder Structure

```
VolunteerBridge/
├── Controllers/          # 9 MVC Controllers
├── Helpers/              # EnumExtensions utility
├── Hubs/                 # ChatHub (SignalR)
├── Migrations/           # 7 EF Core migrations
├── Models/               # 6 entity models + AppDbContext
├── Services/             # EmailService (MailKit)
├── ViewModels/           # 11 ViewModels / DTOs
├── Views/                # Razor views organized by controller
│   └── Shared/           # _Layout, _AdminLayout, _FloatingChat
└── wwwroot/
    ├── css/              # wessal.css, wessal-chat.css, admin.css, progress-tracker.css
    ├── js/               # wessal-chat.js, site.js
    ├── images/           # Static assets
    └── uploads/          # User-uploaded images (runtime)
```

---

## 🔑 Key Workflows

1. **User registers** → Email confirmation via Brevo → Account created
2. **Requester posts** a service request → Appears in Browse feed
3. **Volunteer accepts** → Request marked Accepted → Progress tracker activates
4. **Requester confirms** completion → Points awarded to volunteer
5. **Requester rates** volunteer → Average rating updated
6. **Admin monitors** via dashboard → Can ban users or remove requests

---

## 🗺️ Documentation

| Document | Description |
|----------|-------------|
| [Project Overview](./docs/overview/project-overview.md) | Vision and business problem |
| [System Architecture](./docs/architecture/system-architecture.md) | High-level system design |
| [Database Schema](./docs/database/database-schema.md) | All entities and relationships |
| [Authentication Flow](./docs/authentication/auth-flow.md) | Registration, login, confirmation |
| [SignalR Chat System](./docs/chat-system/signalr-architecture.md) | Real-time chat implementation |
| [Design System](./docs/frontend/design-system.md) | Wessal CSS tokens and components |
| [Local Setup](./docs/development/local-setup.md) | Step-by-step dev environment setup |
| [Deployment Guide](./docs/deployment/deployment-guide.md) | Production deployment checklist |

---

## 👥 Contributors

| Name | Role |
|------|------|
| **Yousef Ehab** | Backend architecture, admin panel, security, UI polish |
| **Merolla Ezzat** | Progress tracking system, request lifecycle |
| **Menna Sayed** | Progress tracking UI, milestone features |
| **Wessal (Design Lead)** | Wessal design system, UI/UX polish |

---

## 🔮 Future Improvements

- [ ] ASP.NET Core Identity migration for proper role-based auth
- [ ] Push notifications (browser/mobile)
- [ ] Real-time request status updates via SignalR
- [ ] Image moderation for uploaded content
- [ ] Mobile-responsive floating chat improvements
- [ ] Admin password stored with BCrypt (currently plaintext in config)
- [ ] Pagination for chat history
- [ ] Geographic map view for requests

---

> Built with ❤️ for the community — وصال
