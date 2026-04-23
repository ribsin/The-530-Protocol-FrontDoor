<!-- [530-PROVENANCE] AI-GENERATED | AGENT_ID: Cline-Agent003-DesignBay | LAST_MODIFIED: 2026-04-23 | SECURITY_STATUS: STEEL-CHECK-PASSED -->

---

[COPILOT_HANDOVER_PROTOCOL]

## Current Objective: **Create Minimal Public Repository with Front Door Only**

## Active Rules:
- Law #10: C# files ≤ 400 lines, markup files ≤ 500 lines
- Law #3: Unified naming scheme (Object_, Service_, I_, Audit_ prefixes)
- Constitutional Shadow-Board: All docs in `/docs` subdirectories

## Context:
The M48 Phase 3 Front Door app (`Project530.Tools.FrontDoor`) is a cross-platform Avalonia UI desktop application that serves as both the "Front Door" installer and live telemetry monitor for the Five30 Factory.

The Five30 Protocol repository is currently private. The goal is to create a **minimal public repository** (`ribsin/The-530-Protocol-FrontDoor`) that exposes ONLY the Front Door app source code and build infrastructure — NOT the full factory codebase (core services, adapters, UI, backend, tests, governance, etc.).

## Current Task List:
> 1. **Create new public repository** — Create `ribsin/The-530-Protocol-FrontDoor` on GitHub (public)
> 2. **Copy minimal files** — Clone new repo locally, add only the public items listed below
> 3. **Push to new public repo** — Push the minimal repository
> 4. **Verify GitHub Release workflow** — CI/CD in new repo must trigger on `frontdoor-v*` tags
> 5. **Update release script** — Modify release workflow to pull source from new public repo

---

## Public Repository Structure

```
The-530-Protocol-FrontDoor/
├── 05_Tools/
│   └── Project530.Tools.FrontDoor/          # The Front Door app (public)
│       ├── Project530.Tools.FrontDoor.csproj
│       ├── Program.cs
│       ├── App.axaml
│       ├── App.axaml.cs
│       ├── Models/
│       ├── Services/
│       ├── ViewModels/
│       └── Views/
├── .github/
│   └── workflows/
│       └── release-frontdoor.yml           # CI/CD for building binaries
├── docs/
│   └── design/
│       └── frontdoor_design.md             # Visual identity SSOT
├── docs/audit/
│   └── COPILOT_HANDOVER_PUBLIC_REPO.md      # This document
├── 01_Core/
│   └── Project530.Core.Common/              # Shared models only (Object_WorkPlanDto, Object_AgentStatusDto, etc.)
├── .gitignore
├── .editorconfig
├── Directory.Build.props
├── global.json
├── LICENSE
└── README.md
```

---

## Files to INCLUDE (Public)

| Path | Purpose | Notes |
|------|--------|-------|
| `05_Tools/Project530.Tools.FrontDoor/` | Front Door app source | Full Avalonia UI application |
| `.github/workflows/release-frontdoor.yml` | CI/CD for releases | Builds Windows/Linux/macOS binaries |
| `docs/design/frontdoor_design.md` | Visual identity SSOT | Color tokens, typography, layout |
| `01_Core/Project530.Core.Common/` | Shared DTOs | Object_WorkPlanDto, Object_AgentStatusDto for SignalR integration |
| `Directory.Build.props` | Solution-wide config | Nullable, implicit usings |
| `global.json` | .NET SDK pinning | Ensures .NET 9.0 |
| `.gitignore` | Standard .NET ignore | bin/, obj/, etc. |
| `LICENSE` | MIT License | Five30 Protocol |
| `README.md` | Public-facing guide | How to build, how to use |

---

## Files to EXCLUDE (Private/Internal)

| Path | Reason |
|------|--------|
| `01_Core/Project530.Core.Domain/` | Internal domain models |
| `01_Core/Project530.Core.Services/` | Internal business logic |
| `02_Security/` | Steel-Check analyzers, security protocols |
| `03_Adapters/` | Game adapters (VRAGE, Vintage Story, etc.) |
| `04_UI/` | Dashboard, Mission Control, backend API |
| `05_Testing/` | Test suites |
| `05_Tools/Project530.Tools.MissionControl/` | WPF IDE (internal tool) |
| `docs/audit/` (except this file) | Internal audit logs |
| `docs/governance/` | Constitutional documents |
| `docs/technical/` | Internal technical specs |
| `scripts/` | Deployment scripts |
| `Five30/`, `vault/`, `infra/`, `milestones/` | Internal configs and data |
| `governance/` | Internal governance |
| `docker-compose.yml` | Internal factory orchestration |

---

## Implementation Steps

### Step 1: Create new repository on GitHub
1. Go to https://github.com/new
2. Create `ribsin/The-530-Protocol-FrontDoor` (public)
3. Do NOT initialize with README

### Step 2: Clone and set up locally
```bash
# Clone new public repo
git clone https://github.com/ribsin/The-530-Protocol-FrontDoor.git
cd The-530-Protocol-FrontDoor

# Copy Core.Common (shared DTOs)
mkdir -p 01_Core/Project530.Core.Common
cp -r ../The-530-Protocol/01_Core/Project530.Core.Common/Models/ 01_Core/Project530.Core.Common/

# Copy Front Door app
mkdir -p 05_Tools
cp -r ../The-530-Protocol/05_Tools/Project530.Tools.FrontDoor/ 05_Tools/

# Copy design doc
mkdir -p docs/design
cp ../The-530-Protocol/docs/design/frontdoor_design.md docs/design/

# Copy CI/CD workflow
mkdir -p .github/workflows
cp ../The-530-Protocol/.github/workflows/release-frontdoor.yml .github/workflows/

# Copy config files
cp ../The-530-Protocol/Directory.Build.props ./
cp ../The-530-Protocol/global.json ./
cp ../The-530-Protocol/.editorconfig ./
cp ../The-530-Protocol/.gitignore ./
cp ../The-530-Protocol/LICENSE ./

# Create public README
cp ../The-530-Protocol/README.md ./
# Edit README.md to reference Front Door only
```

### Step 3: Update README.md for public repo
Replace README content with public-facing guide:
```markdown
# Five30 Front Door

Cross-platform installer and live monitor for the Five30 Factory.

## Features

- **Setup Wizard** — Bootstrap the Five30 Factory on a fresh Ubuntu machine
- **Live Monitor** — Real-time telemetry dashboard with 5 tabs
- **Cross-Platform** — Windows, Linux, and macOS binaries via CI/CD

## Building

```bash
dotnet build Project530.Tools.FrontDoor.csproj
```

## Download

Pre-built binaries are available on the [GitHub Releases](https://github.com/ribsin/The-530-Protocol-FrontDoor/releases) page.

## System Requirements

- .NET 9.0 (or use self-contained binaries)
- Ubuntu 20.04+ / Windows 10+ / macOS 12+
- Docker (for running the factory)
```

### Step 4: Push to public repo
```bash
git add .
git commit -m "Initial public release: Five30 Front Door"
git branch -M main
git push -u origin main
```

### Step 5: Tag and release
```bash
git tag frontdoor-v1.0.0
git push origin frontdoor-v1.0.0
```

---

## Verification Checklist

- [ ] New repo is public
- [ ] Only Front Door source code + build infra included
- [ ] `dotnet build Project530.Tools.FrontDoor.csproj` succeeds locally
- [ ] CI/CD workflow triggers on `frontdoor-v*` tag
- [ ] GitHub Release shows 3 platform binaries
- [ ] README.md references the public release URL
- [ ] No internal files (core services, adapters, tests, docs/governance, etc.) visible

---

## Notes for Foreman

- The `01_Core/Project530.Core.Common` contains ONLY shared DTOs (read-only data models) — no business logic
- All model classes use `[530-PROVENANCE]` headers
- The release workflow is unchanged from the main repo
- The new public repo URL will be: `https://github.com/ribsin/The-530-Protocol-FrontDoor`

