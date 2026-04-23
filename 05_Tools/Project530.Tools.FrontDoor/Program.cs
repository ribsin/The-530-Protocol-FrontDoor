// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-23 | SESSION: M48-HeadlessMode
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Program.cs (CLI Router)
 * *************************************************************************** */

using Avalonia;
using Avalonia.ReactiveUI;
using Project530.Tools.FrontDoor;
using Project530.Tools.FrontDoor.Services;

// ── CLI routing ──────────────────────────────────────────────────────────────
// Supports three modes:
//   GUI mode     : frontdoor                            (default, requires display)
//   Server mode  : frontdoor server <command>           (single command, exits)
//   Headless mode: frontdoor --headless                 (interactive REPL, no display)
// Auto-switches to headless when no display is detected.
// ─────────────────────────────────────────────────────────────────────────────

if (args.Length >= 2 && string.Equals(args[1], "server", StringComparison.OrdinalIgnoreCase))
{
    // Single-command server mode: frontdoor server <status|start|stop|restart|update|logs|health>
    var svc = Service_HeadlessMode.CreateDefault();
    var command = args.Length >= 3 ? args[2] : "status";
    var output = await svc.RunCommandAsync(command, CancellationToken.None);
    Console.WriteLine(output);
    return;
}

bool headlessFlag = Array.Exists(args, a =>
    string.Equals(a, "--headless", StringComparison.OrdinalIgnoreCase) ||
    string.Equals(a, "-s", StringComparison.OrdinalIgnoreCase));

if (headlessFlag || IsHeadlessEnvironment())
{
    var svc = Service_HeadlessMode.CreateDefault();
    await svc.RunInteractiveAsync(CancellationToken.None);
    return;
}

// ── GUI mode (Avalonia) ───────────────────────────────────────────────────────
AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .LogToTrace()
    .UseReactiveUI()
    .StartWithClassicDesktopLifetime(args);

// ── Helpers ───────────────────────────────────────────────────────────────────
static bool IsHeadlessEnvironment()
{
    // SSH session
    if (Environment.GetEnvironmentVariable("SSH_CONNECTION") != null)
        return true;

    // tmux / screen
    if (Environment.GetEnvironmentVariable("TMUX") != null)
        return true;

    // No X11 display on Linux/macOS
    if (Environment.GetEnvironmentVariable("DISPLAY") == null
        && !OperatingSystem.IsWindows())
        return true;

    // WSL without a display server
    if (Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null
        && Environment.GetEnvironmentVariable("DISPLAY") == null)
        return true;

    return false;
}
