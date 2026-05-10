# وصال (Wessal) — Platform Redesign Walkthrough

## What Was Done

Successfully generated **9 complete UI screens** via Google Stitch for the وصال volunteer platform, all adhering to the **Ethos Modern** design system.

### Design System Applied
| Token | Value |
|-------|-------|
| **Primary** | `#004ac6` (Blue) |
| **Secondary** | `#006e2f` (Green) |
| **Surface** | `#f7f9fb` |
| **Text** | `#191c1e` / `#434655` |
| **Font** | IBM Plex Sans Arabic |
| **Radius** | 8px (buttons), 12px (cards) |
| **Shadows** | 4-8% opacity, ambient only |
| **Direction** | RTL (Arabic-first) |

---

## Generated Screens

### Phase 1 — The Gateway
| Screen | Stitch Title | Key Features |
|--------|-------------|--------------|
| **Landing Page** | وصال - الصفحة الرئيسية | Asymmetric hero, "كيف تعمل وصال" steps, stats section, footer |
| **Login** | وصال - تسجيل الدخول | Split-screen (form + blue gradient visual), RTL inputs, brand accent |
| **Register** | وصال - إنشاء حساب جديد | 6-field form, blue-to-green gradient visual, helper text |

### Phase 2 — Volunteer Experience
| Screen | Stitch Title | Key Features |
|--------|-------------|--------------|
| **Browse Requests** | وصال - تصفح طلبات التطوع | Filter bar, horizontal list cards, status badges, pagination |
| **Request Details** | وصال - تفاصيل طلب التطوع | Breadcrumb, detail card, volunteer CTA, acceptances list |
| **Volunteer Profile** | وصال - ملف المتطوع | Avatar initials, stats row, activity tabs, request table |

### Phase 3 — Admin Experience
| Screen | Stitch Title | Key Features |
|--------|-------------|--------------|
| **Admin Dashboard** | وصال - لوحة التحكم الإدارية | Dark sidebar, 7-metric stats grid, top volunteers table, recent requests |
| **Admin Users** | وصال - إدارة المستخدمين | Search + filter, user table with ban/unban actions |
| **Admin Requests** | وصال - إدارة الطلبات | Status filters, request table with view/delete, inline stats summary |

---

## Project Reference
- **Stitch Project ID**: `3172510147459699114`
- **Design System Asset**: `assets/0ed0ddb4f7c54833937f174dcba76758`
- **Stitch URL**: [View in Stitch](https://stitch.withgoogle.com/projects/3172510147459699114)

---

## Next Steps

> [!IMPORTANT]
> The Stitch screens are design references. The next phase is to **extract the HTML** from each screen and adapt it into the existing ASP.NET Razor views, preserving all server-side logic (`@model`, `@Html.*`, Session checks).

1. **Download HTML** from each Stitch screen via the `get_screen` API
2. **Create `wessal-design-system.css`** — a unified CSS file with all design tokens
3. **Migrate views** — Replace existing `.cshtml` content with the new HTML, wrapping dynamic data with Razor syntax
4. **Test locally** — Run `dotnet run` and verify RTL rendering, form submissions, and admin flows
