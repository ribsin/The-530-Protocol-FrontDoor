# Five30 Front Door

Cross-platform installer and live monitor for the Five30 Factory.

## Features

- Setup Wizard: Bootstrap the Five30 Factory on a fresh Ubuntu machine
- Live Monitor: Real-time telemetry dashboard
- Cross-Platform: Windows, Linux, and macOS binaries via CI/CD

## Building

```bash
dotnet build 05_Tools/Project530.Tools.FrontDoor/Project530.Tools.FrontDoor.csproj
```

## Download

Pre-built binaries are available on the GitHub Releases page:
https://github.com/ribsin/The-530-Protocol-FrontDoor/releases

## Release Tags

Use tags in this format to trigger CI release builds:

- frontdoor-v1.0.0
- frontdoor-v1.1.0

## System Requirements

- .NET 9.0 SDK (for local build)
- Ubuntu 20.04+ / Windows 10+ / macOS 12+
- Docker (for running the factory stack)
