# Design System — وصال (Wessal)

## Overview

The Wessal design system is a **custom, RTL-first CSS system** built entirely in `wessal.css` (1174 lines). It uses CSS custom properties (design tokens) and utility classes prefixed with `w-`. Bootstrap 5 is included as a baseline but is largely overridden.

The system is inspired by **Material Design 3** (color system, surface hierarchy) combined with a modern **glassmorphism** aesthetic.

---

## Typography

Two fonts are imported from Google Fonts:

```css
@import url('https://fonts.googleapis.com/css2?family=Rubik:wght@300;400;500;600;700;800;900&family=IBM+Plex+Sans+Arabic:wght@300;400;500;600;700&display=swap');
```

| Font | Variable | Usage |
|------|----------|-------|
| **Rubik** | `--w-font-display` | Headings, brand name, display text |
| **IBM Plex Sans Arabic** | `--w-font-body` | Body text, inputs, labels |

### Type Scale

| Class | Size | Weight | Usage |
|-------|------|--------|-------|
| `.w-display` | 56px | 800 | Hero headlines |
| `.w-h1` | 40px | 700 | Page titles |
| `.w-h2` | 32px | 700 | Section titles |
| `.w-h3` | 28px | 600 | Subsection titles |
| `.w-h4` | 24px | 600 | Card headers |
| `.w-h5` | 20px | 600 | Component headers |
| `.w-h6` | 18px | 600 | Small headers |
| `.w-body-lg` | 18px | 400 | Long-form body |
| `.w-body-md` | 16px | 400 | Default body |
| `.w-body-sm` | 14px | 400 | Secondary text |
| `.w-caption` | 12px | 500 | Labels, tags (uppercase) |
| `.w-label` | 14px | 600 | Form labels |

---

## Color Palette — "Aurora"

All colors are CSS custom properties defined on `:root`:

### Primary (Indigo)
```css
--w-primary: #6366f1;        /* Indigo 500 — CTAs, active nav, links */
--w-primary-hover: #4f46e5;  /* Indigo 600 — hover state */
--w-primary-container: rgba(99, 102, 241, 0.1); /* Tinted backgrounds */
--w-on-primary: #ffffff;
--w-on-primary-container: #312e81;
```

### Secondary (Emerald)
```css
--w-secondary: #10b981;       /* Emerald 500 — success, completion */
--w-secondary-hover: #059669; /* Emerald 600 */
--w-secondary-container: rgba(16, 185, 129, 0.1);
--w-on-secondary: #ffffff;
```

### Accent (Rose)
```css
--w-accent: #f43f5e;          /* Rose 500 — danger, destructive */
--w-accent-hover: #e11d48;
```

### Surfaces
```css
--w-surface: #ffffff;
--w-surface-dim: #f8fafc;
--w-surface-container: #f1f5f9;
--w-surface-container-low: #f8fafc;
--w-surface-container-lowest: #ffffff;
--w-surface-container-highest: #e2e8f0;
--w-on-surface: #0f172a;
--w-on-surface-variant: #64748b;
--w-outline: #e2e8f0;
--w-outline-hover: #cbd5e1;
```

### Glassmorphism Variables
```css
--w-glass-bg: rgba(255, 255, 255, 0.65);
--w-glass-border: rgba(255, 255, 255, 0.4);
--w-glass-blur: 16px;
--w-glass-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.07);
```

---

## Spacing System

```css
--w-space-xs: 4px;
--w-space-sm: 8px;
--w-space-md: 16px;
--w-space-lg: 24px;
--w-space-xl: 32px;
--w-space-2xl: 48px;
--w-space-3xl: 64px;
--w-container-max: 1200px;
```

Spacing utility classes follow the pattern `w-{property}-{size}`:
- `w-mt-md`, `w-mb-xl`, `w-py-lg`, `w-px-sm`, `w-gap-lg`, etc.

---

## Border Radius System

```css
--w-radius-sm: 8px;
--w-radius: 12px;
--w-radius-md: 16px;
--w-radius-lg: 24px;
--w-radius-xl: 32px;
--w-radius-full: 9999px;  /* Pill / circle */
```

---

## Shadow System

```css
--w-shadow-sm: 0 1px 2px 0 rgb(0 0 0 / 0.05);
--w-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1);
--w-shadow-md: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);
--w-shadow-lg: 0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1);
--w-shadow-xl: 0 25px 50px -12px rgb(0 0 0 / 0.1);
```

---

## Core Components

### Buttons

| Class | Style | Usage |
|-------|-------|-------|
| `.w-btn.w-btn-primary` | Solid indigo with shadow | Primary CTAs |
| `.w-btn.w-btn-secondary` | Solid emerald | Success actions |
| `.w-btn.w-btn-outline` | Bordered indigo | Secondary CTAs |
| `.w-btn.w-btn-ghost` | Transparent | Tertiary actions |
| `.w-btn.w-btn-danger` | Solid rose | Destructive actions |
| `.w-btn.w-btn-outline-danger` | Bordered rose | Soft destructive |

Size modifiers: `.w-btn-sm`, `.w-btn-lg`, `.w-btn-full`

All buttons have:
- `transition: 300ms cubic-bezier(0.4, 0, 0.2, 1)`
- `translateY(-2px)` lift on hover
- Glow box-shadow on hover

### Cards

| Class | Description |
|-------|-------------|
| `.w-card` | White card with border and shadow |
| `.w-card-glass` | Glassmorphism card (60% opacity, blur) |

Cards have `translateY(-4px)` on hover with elevated shadow.

### Form Inputs (`.w-input`)
- Full-width, RTL text align
- Focus: indigo border + `box-shadow: 0 0 0 4px primary-container`
- Error state: `.w-input-error` with rose border

### Chips / Badges (`.w-chip`)

| Class | Color |
|-------|-------|
| `.w-chip-green` | Emerald |
| `.w-chip-blue` | Indigo |
| `.w-chip-red` | Rose |
| `.w-chip-gray` | Neutral |
| `.w-chip-glass` | Glassmorphism (for dark backgrounds) |

### Alerts (`.w-alert`)
- `.w-alert-success` — Emerald tinted
- `.w-alert-error` — Rose tinted
- Include a close button (`.w-alert-close`)

### Navbar (`.w-navbar`)
- Fixed, glassmorphism (70% opacity, 16px blur)
- Becomes more opaque on scroll (`.scrolled` class via JS)
- Collapses nav links on mobile (< 768px)

### Avatars (`.w-avatar`)
- Circular, initials-based (no image uploads for profiles)
- Sizes: `.w-avatar-sm` (32px) → `.w-avatar-xl` (80px)

---

## RTL Support

RTL is implemented at the **HTML root level**:

```css
html {
    direction: rtl;
    text-align: right;
}
```

All inputs have `direction: rtl; text-align: right;`. Flex-direction and margin/padding utilities are defined symmetrically for RTL. Bootstrap's RTL grid is used.

Key RTL-specific utility:
```css
.w-text-right { text-align: right; }
.w-text-left { text-align: left; }
```

---

## Animation System

```css
/* Standard easing across all interactive elements */
--w-transition: 300ms cubic-bezier(0.4, 0, 0.2, 1);

/* Keyframes */
@keyframes w-fadeInUp { from { opacity: 0; transform: translateY(20px); } to { ... } }
@keyframes w-fadeIn { from { opacity: 0; } to { opacity: 1; } }
@keyframes w-slideInRight { ... }
@keyframes w-pulse { ... }    /* For loading states */
@keyframes w-glow { ... }     /* For active progress stage */
```

---

## Body Background

The background is a subtle **multi-radial gradient** (aurora effect) fixed to the viewport:

```css
body {
    background-image: 
        radial-gradient(at 10% 10%, rgba(99, 102, 241, 0.15) 0px, transparent 40%),
        radial-gradient(at 90% 10%, rgba(16, 185, 129, 0.1) 0px, transparent 40%),
        radial-gradient(at 90% 90%, rgba(244, 63, 94, 0.1) 0px, transparent 40%),
        radial-gradient(at 10% 90%, rgba(99, 102, 241, 0.1) 0px, transparent 40%),
        radial-gradient(at 50% 50%, rgba(16, 185, 129, 0.05) 0px, transparent 50%);
    background-attachment: fixed;
}
```

---

## CSS File Architecture

| File | Size | Purpose |
|------|------|---------|
| `wessal.css` | ~34KB | Core design system — tokens, typography, components, utilities |
| `wessal-chat.css` | ~12KB | Floating chat widget styles |
| `admin.css` | ~18KB | Admin panel specific styles (sidebar, KPI cards, tables) |
| `progress-tracker.css` | ~4KB | 7-stage timeline progress component |
| `site.css` | ~2KB | Legacy minimal styles (mostly overridden) |

> **Note:** There is no build system (no Sass, PostCSS, or webpack). All CSS is plain vanilla and loaded directly. This is appropriate for the project's scale but would benefit from a build step for production minification.

---

## Special Visual Effects

### Glassmorphism
Used on: navbar, card variants, chat widget, leaderboard rows
```css
.w-glass {
    background: rgba(255, 255, 255, 0.65);
    backdrop-filter: blur(16px);
    -webkit-backdrop-filter: blur(16px);
    border: 1px solid rgba(255, 255, 255, 0.4);
}
```

### Aurora Text Gradient
Used on brand name and hero headlines:
```css
.w-aurora-text {
    background: linear-gradient(135deg, var(--w-primary) 0%, var(--w-secondary) 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
}
```

### Leaderboard Trophy Cards
Gold/Silver/Bronze gradient cards with hover lift and glow effects for top 3 volunteers.
