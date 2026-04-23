// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-23 | SESSION: M48-HeadlessMode
 * SECURITY_STATUS: STEEL-CHECK-PASSED (System.IO allowed in Tools tier)
 * DOCUMENT: Front Door — Service_HeadlessMode (Terminal REPL + single-command)
 * *************************************************************************** */

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Provides a headless terminal interface for managing the Five30 factory stack
/// from SSH sessions, tmux, or any environment without a display server.
///
/// Usage (interactive): frontdoor --headless
/// Usage (single cmd) : frontdoor server &lt;command&gt;
/// </summary>
public sealed class Service_HeadlessMode
{
    private const string Prompt = "five30> ";

    private readonly Service_BackendController _controller;
    private readonly Service_ProcessRunner _runner;
    private readonly string _repoPath;

    private static readonly string[] HelpLines =
    {
        "",
        "  Five30 Front Door — Headless Mode",
        "  ─────────────────────────────────",
        "  status   Show backend container status",
        "  start    Start docker-compose stack",
        "  stop     Stop docker-compose stack",
        "  restart  Restart docker-compose stack",
        "  update   Git pull + rebuild backend",
        "  logs     Stream container logs (Ctrl+C to stop)",
        "  health   Ping backend health endpoint",
        "  help     Show this message",
        "  exit     Quit",
        "",
    };

    public Service_HeadlessMode(
        Service_BackendController controller,
        Service_ProcessRunner runner,
        string repoPath)
    {
        _controller = controller;
        _runner = runner;
        _repoPath = repoPath;
    }

    /// <summary>Creates a default instance using FIVE30_REPO_PATH or current directory.</summary>
    public static Service_HeadlessMode CreateDefault()
    {
        var repoPath = Environment.GetEnvironmentVariable("FIVE30_REPO_PATH")
            ?? Directory.GetCurrentDirectory();

        var runner = new Service_ProcessRunner();
        var controller = new Service_BackendController(repoPath, runner);
        return new Service_HeadlessMode(controller, runner, repoPath);
    }

    // ── Interactive REPL ─────────────────────────────────────────────────────

    /// <summary>Runs an interactive terminal REPL until the user types exit/quit.</summary>
    public async Task RunInteractiveAsync(CancellationToken ct)
    {
        PrintBanner();

        while (!ct.IsCancellationRequested)
        {
            Console.Write(Prompt);

            // Guard: Console.ReadLine can throw in redirect/non-TTY scenarios.
            string? input;
            try
            {
                input = Console.ReadLine();
            }
            catch (IOException)
            {
                break;
            }

            if (input == null)
                break;

            input = input.Trim();

            if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase)
                || string.Equals(input, "quit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Goodbye.");
                break;
            }

            if (string.IsNullOrEmpty(input))
                continue;

            if (string.Equals(input, "logs", StringComparison.OrdinalIgnoreCase))
            {
                await StreamLogsAsync(ct);
                continue;
            }

            var result = await RunCommandAsync(input, ct);
            Console.WriteLine(result);
        }
    }

    // ── Single-command execution ─────────────────────────────────────────────

    /// <summary>Executes a single named command and returns its output string.</summary>
    public async Task<string> RunCommandAsync(string command, CancellationToken ct)
    {
        switch (command.ToLowerInvariant())
        {
            case "status":
                return await GetStatusTextAsync(ct);

            case "start":
                var startResult = await _controller.StartAsync(ct);
                return startResult.IsSuccess ? "Stack started." : $"Start failed: {startResult.ErrorMessage}";

            case "stop":
                var stopResult = await _controller.StopAsync(ct);
                return stopResult.IsSuccess ? "Stack stopped." : $"Stop failed: {stopResult.ErrorMessage}";

            case "restart":
                var restartResult = await _controller.RestartAsync(ct);
                return restartResult.IsSuccess ? "Stack restarted." : $"Restart failed: {restartResult.ErrorMessage}";

            case "update":
                var updateResult = await _controller.UpdateAsync(ct);
                return updateResult.IsSuccess ? "Update complete." : $"Update failed: {updateResult.ErrorMessage}";

            case "health":
                return await GetHealthTextAsync(ct);

            case "help":
            case "--help":
            case "-h":
                return string.Join(Environment.NewLine, HelpLines);

            default:
                return $"Unknown command '{command}'. Type 'help' for available commands.";
        }
    }

    // ── Streaming logs ───────────────────────────────────────────────────────

    private async Task StreamLogsAsync(CancellationToken ct)
    {
        Console.WriteLine("[Streaming logs — press Ctrl+C to stop]");
        try
        {
            await foreach (var line in _runner.RunAsync(
                "docker", "compose logs --follow", _repoPath, ct))
            {
                Console.WriteLine(line);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[Log stream ended]");
        }
    }

    // ── Status helpers ───────────────────────────────────────────────────────

    private async Task<string> GetStatusTextAsync(CancellationToken ct)
    {
        var status = await _controller.GetStatusAsync(ct);
        return $"Backend status: {status}  (repo: {_repoPath})";
    }

    private async Task<string> GetHealthTextAsync(CancellationToken ct)
    {
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await http.GetAsync("http://localhost:5000/health", ct);
            return response.IsSuccessStatusCode
                ? $"Health: OK ({(int)response.StatusCode})"
                : $"Health: degraded ({(int)response.StatusCode})";
        }
        catch (Exception ex)
        {
            return $"Health: unreachable ({ex.Message})";
        }
    }

    // ── Banner ───────────────────────────────────────────────────────────────

    private static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("  ┌─────────────────────────────────────┐");
        Console.WriteLine("  │  Five30 Front Door  —  Headless Mode │");
        Console.WriteLine("  │  Type 'help' for available commands  │");
        Console.WriteLine("  │  Type 'exit' to quit                 │");
        Console.WriteLine("  └─────────────────────────────────────┘");
        Console.WriteLine();
        Console.ResetColor();
    }
}
