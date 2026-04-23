// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-23 | SESSION: M49-AutoDetect
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Program.cs (CLI Router)
 * *************************************************************************** */

using Avalonia;
using Avalonia.ReactiveUI;
using Project530.Tools.FrontDoor;
using Project530.Tools.FrontDoor.Services;

// ── CLI routing ──────────────────────────────────────────────────────────────
// Supports four modes:
//   GUI mode       : frontdoor [--gui|-g]               (explicit or auto, requires display)
//   Server mode    : frontdoor server <command>         (single command, exits)
//   Headless mode  : frontdoor [--headless|-s]          (explicit interactive REPL)
//   Auto-detect    : frontdoor                          (headless if no display found)
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

bool forceGui = Array.Exists(args, a =>
    string.Equals(a, "--gui", StringComparison.OrdinalIgnoreCase) ||
    string.Equals(a, "-g", StringComparison.OrdinalIgnoreCase));

bool headlessFlag = Array.Exists(args, a =>
    string.Equals(a, "--headless", StringComparison.OrdinalIgnoreCase) ||
    string.Equals(a, "-s", StringComparison.OrdinalIgnoreCase));

bool autoHeadless = !forceGui && HeadlessDetector.IsHeadlessEnvironment();

if (headlessFlag || autoHeadless)
{
    var svc = Service_HeadlessMode.CreateDefault();
    await svc.RunInteractiveAsync(CancellationToken.None);
    return;
}

// ── Dependency Check (Linux only) ─────────────────────────────────────────────
// Check for GTK3/X11 dependencies before attempting GUI launch
if (!HeadlessDetector.IsHeadlessEnvironment())
{
    var depChecker = new Service_DependencyChecker();
    depChecker.ShowDependencyPrompt();
    
    // If user chose headless mode, run that instead
    if (Environment.GetEnvironmentVariable("FIVE30_HEADLESS_MODE") == "1")
    {
        var svc = Service_HeadlessMode.CreateDefault();
        await svc.RunInteractiveAsync(CancellationToken.None);
        return;
    }
}

// ── GUI mode (Avalonia) ───────────────────────────────────────────────────────
// If --gui was passed on a machine with no display, report clearly and exit.
if (forceGui && HeadlessDetector.IsHeadlessEnvironment())
{
    Console.Error.WriteLine("[Five30] GUI mode requested but no display is available.");
    Console.Error.WriteLine("         Remove --gui to use auto-detect, or set up X11 forwarding.");
    return;
}

try
{
    AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .LogToTrace()
        .UseReactiveUI()
        .StartWithClassicDesktopLifetime(args);
}
catch (Exception ex) when (
    ex.Message.IndexOf("display", StringComparison.OrdinalIgnoreCase) >= 0 ||
    ex.Message.IndexOf("gtk", StringComparison.OrdinalIgnoreCase) >= 0 ||
    ex.Message.IndexOf("screen", StringComparison.OrdinalIgnoreCase) >= 0)
{
    Console.WriteLine("[Five30] No display available — switching to headless mode...");
    var svc = Service_HeadlessMode.CreateDefault();
    await svc.RunInteractiveAsync(CancellationToken.None);
}
