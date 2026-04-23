<!-- [530-PROVENANCE] AI-GENERATED | AGENT_ID: Cline-Agent003-DesignBay | LAST_MODIFIED: 2026-04-23 | SECURITY_STATUS: STEEL-CHECK-PASSED -->

# Front Door — Visual Identity (Single Source of Truth)

> **[CONSTITUTIONAL LAW]** This document is the **SINGLE CANONICAL SOURCE** for the `Project530.Tools.FrontDoor` Avalonia UI app's visual design, layout architecture, and color system. All view changes must conform to this spec.
>
> **Exemption:** The Front Door app is **exempt** from the website's `--m11-` Kinetic Terminal green-phosphor scheme (per Manager Design Override).

---

## 1. Application Overview

**Application:** `05_Tools/Project530.Tools.FrontDoor`  
**Framework:** Avalonia UI 11.x with ReactiveUI / CommunityToolkit.Mvvm  
**Purpose:** Cross-platform "Front Door" installer + live telemetry monitor for the Five30 Factory

### 1.1 Two Modes

| Mode | Trigger | Purpose |
|---|---|---|
| **Setup Wizard** | Backend not reachable at port 5000 | 7-step factory bootstrap (git clone, docker compose, vault keys) |
| **Live Monitor** | Factory healthy | Real-time telemetry dashboard with 5 tabs |

---

## 2. Command Deck Visual Identity

### 2.1 Design Rationale
- **Mood:** A ship's bridge or mission control deck — focused, professional, high-contrast
- **Differentiation:** Blue-accented instead of green (website's phosphor scheme); softer on eyes for long monitoring sessions
- **Hardware Feel:** Retains notched corners and 0px radius panels (brutalist DNA), but with blue neon borders

### 2.2 Color Palette

| Token | Hex | Purpose |
|---|---|---|
| `--fd-bg-void` | `#0a0e17` | Deepest background — deep navy/black |
| `--fd-bg-chassis` | `#111827` | Primary surface — gunmetal blue-gray |
| `--fd-bg-panel` | `#1f2937` | Elevated panels — slate gray |
| `--fd-bg-card` | `#1e2435` | Card surfaces |
| `--fd-bg-panel-raised` | `#374151` | Hover/focus states |
| `--fd-accent-primary` | `#3b82f6` | Primary interactive — electric blue |
| `--fd-accent-bright` | `#60a5fa` | Hero headings, CTAs |
| `--fd-accent-glow` | `rgba(59, 130, 246, 0.25)` | Box-shadow glow layer |
| `--fd-accent-success` | `#22c55e` | Healthy/online indicators |
| `--fd-accent-warning` | `#f59e0b` | Caution states |
| `--fd-accent-danger` | `#ef4444` | Error/warning states |
| `--fd-text-primary` | `#e5e7eb` | Main body text — soft white |
| `--fd-text-secondary` | `#9ca3af` | Supporting text |
| `--fd-text-muted` | `#6b7280` | Timestamps, IDs |
| `--fd-border` | `#1e2d47` | Subtle borders |

### 2.3 Typography

- **Headings:** `Rajdhani` (ALL CAPS, tracking 0.12em) — consistency with factory branding
- **Body:** `Segoe UI` (Windows) / `Inter` (Linux/macOS)
- **Data & Logs:** `JetBrains Mono` or `Cascadia Mono` — mandatory monospace for alignment

---

## 3. Panel System

### 3.1 Layout Rules
- **Border:** 1px solid `--fd-accent-primary`, no rounded corners (`CornerRadius="0"`)
- **Corner Notches:** 8x8px clipped corners via `PathGeometry` or `Clip` — mechanical hardware feel
- **Background:** `--fd-bg-card` with subtle 1px inner border
- **Hover Glow:** `--fd-accent-glow` box-shadow on interactive elements

### 3.2 XAML Theme Resources

```xml
<!-- App.axaml -->
<Application.Resources>
    <SolidColorBrush x:Key="FD_BgVoid"        Color="#0a0e17"/>
    <SolidColorBrush x:Key="FD_BgChassis"      Color="#111827"/>
    <SolidColorBrush x:Key="FD_BgPanel"       Color="#1f2937"/>
    <SolidColorBrush x:Key="FD_BgCard"         Color="#1e2435"/>
    <SolidColorBrush x:Key="FD_BgPanelRaised"  Color="#374151"/>
    <SolidColorBrush x:Key="FD_AccentPrimary"  Color="#3b82f6"/>
    <SolidColorBrush x:Key="FD_AccentBright"   Color="#60a5fa"/>
    <SolidColorBrush x:Key="FD_AccentSuccess"  Color="#22c55e"/>
    <SolidColorBrush x:Key="FD_AccentWarning"  Color="#f59e0b"/>
    <SolidColorBrush x:Key="FD_AccentDanger"   Color="#ef4444"/>
    <SolidColorBrush x:Key="FD_TextPrimary"    Color="#e5e7eb"/>
    <SolidColorBrush x:Key="FD_TextMuted"       Color="#6b7280"/>
    <SolidColorBrush x:Key="FD_Border"         Color="#1e2d47"/>
</Application.Resources>
```

---

## 4. Views & Layouts

### 4.1 MainWindow (Shell)
- 960×620 default size, 800×520 minimum
- Mode toggle: `SetupWizardView` vs `LiveMonitorView`
- Dark chassis background fills entire window

### 4.2 SetupWizardView
- Scrollable single-panel layout with step indicator
- Progress bar tracks installation steps (0-100%)
- Log scroll panel shows real-time output
- Input fields for: GitHub username/token, Vault API key, DB password, target directory

### 4.3 LiveMonitorView
- Tab strip: Live Ops, Workforce, Compliance, Brain Pool
- Top status bar with health indicator
- Footer with token balance, traffic rate, backend URL
- Footer always visible across all tabs

### 4.4 Panel Views
- **Panel_Compliance.axaml** — Law compliance cards with progress bars
- **Panel_Infrastructure.axaml** — Host metrics, Docker containers, services status

---

## 5. Brand Elements

| Element | Specification |
|---|---|
| **App Title** | "Five30 Front Door" |
| **Header Color** | `--fd-accent-primary` (#3b82f6) |
| **Success Indicator** | `--fd-accent-success` (#22c55e) |
| **Default Directory** | `/opt/project530` (Linux) or `~/project530` (Windows) |

---

*Document maintained by Agent #003 (Design Bay — Cline).  
Supersedes `docs/design/remote_monitor_design.md`.*
