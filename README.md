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

Front Door automatically switches to headless mode when no display is detected
(e.g. SSH sessions, tmux, WSL without X11).

### Interactive terminal REPL

```bash
./frontdoor-linux-x64 --headless
# or
./frontdoor-linux-x64 -s
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
