# Five30 Front Door

Cross-platform installer, live monitor, and remote server manager for the Five30 Factory.

## Features

- **Setup Wizard** — Bootstrap the Five30 Factory on a fresh Ubuntu machine
- **Live Monitor** — Real-time telemetry dashboard
- **Headless / Server Mode** — Manage your factory over SSH with no display required
- **Cross-Platform** — Windows, Linux, and macOS binaries via CI/CD

## Download

Pre-built binaries are available on the GitHub Releases page:
https://github.com/ribsin/The-530-Protocol-FrontDoor/releases

## Headless / Server Mode

Front Door **automatically** switches to headless terminal mode when no display is
detected (e.g. double-click on a headless Ubuntu server, SSH sessions, tmux, WSL
without X11).  No flags required.

### Behaviour by invocation

| How to run | Result |
|------------|--------|
| Double-click binary (no display) | Auto-detects → terminal REPL |
| `./frontdoor` via SSH | Auto-detects → terminal REPL |
| `./frontdoor --headless` or `-s` | Forces headless REPL |
| `./frontdoor --gui` or `-g` | Forces GUI mode (fails gracefully on headless) |
| `./frontdoor` with a display | Opens Avalonia GUI |
| `./frontdoor server <cmd>` | Single command, exits |

### Interactive terminal REPL

```bash
./frontdoor-linux-x64          # auto-detect on headless machine
./frontdoor-linux-x64 --headless
```

Launches an interactive `five30>` prompt with the following commands:

| Command   | Description                          |
|-----------|--------------------------------------|
| `status`  | Show backend container status        |
| `start`   | Start docker-compose stack           |
| `stop`    | Stop docker-compose stack            |
| `restart` | Restart docker-compose stack         |
| `update`  | Git pull + rebuild backend           |
| `logs`    | Stream container logs (Ctrl+C stops) |
| `health`  | Ping backend health endpoint         |
| `help`    | Show available commands              |
| `exit`    | Quit                                 |

### Single command (scripting / cron)

```bash
./frontdoor-linux-x64 server status
./frontdoor-linux-x64 server restart
./frontdoor-linux-x64 server update
```

### Environment variable

Set `FIVE30_REPO_PATH` to the directory containing your `docker-compose.yml`:

```bash
export FIVE30_REPO_PATH=/opt/five30
./frontdoor-linux-x64 server start
```

If unset, the current working directory is used.

## Building Locally

```bash
dotnet build 05_Tools/Project530.Tools.FrontDoor/Project530.Tools.FrontDoor.csproj
```

## Release Tags

Tags in this format trigger CI builds that publish all three platform binaries:

```
frontdoor-v1.0.2
```

## System Requirements

- .NET 9.0 SDK (for local build; release binaries are self-contained)
- Ubuntu 20.04+ / Windows 10+ / macOS 12+
- Docker (for running the factory stack)
