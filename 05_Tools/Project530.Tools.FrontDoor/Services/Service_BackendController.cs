// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST-MODIFIED: 2026-04-23 | SESSION: M48-Gap-Resolution
 * SECURITY_STATUS: STEEL-CHECK-PASSED (Server-side, System.IO ALLOWED)
 * *************************************************************************** */

using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Full backend lifecycle management: Start, Stop, Restart, Update, Status.
/// </summary>
public sealed class Service_BackendController
{
    private readonly string _repoPath;
    private readonly I_ProcessRunner _processRunner;

    public Service_BackendController(
        string repoPath,
        I_ProcessRunner processRunner)
    {
        _repoPath = repoPath;
        _processRunner = processRunner;
    }

    public async Task<Result> StartAsync(CancellationToken ct)
    {
        await foreach (var _ in _processRunner.RunAsync(
            "docker", "compose up -d --remove-orphans", _repoPath, ct))
        {
            // Stream to log if needed
        }
        return Result.Ok();
    }

    public async Task<Result> StopAsync(CancellationToken ct)
    {
        await foreach (var _ in _processRunner.RunAsync(
            "docker", "compose down", _repoPath, ct))
        {
            // Stream to log if needed
        }
        return Result.Ok();
    }

    public async Task<Result> RestartAsync(CancellationToken ct)
    {
        await foreach (var _ in _processRunner.RunAsync(
            "docker", "compose restart", _repoPath, ct))
        {
            // Stream to log if needed
        }
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(CancellationToken ct)
    {
        // 1. Git pull latest
        await foreach (var _ in _processRunner.RunAsync(
            "git", "pull --ff-only", _repoPath, ct))
        {
            // Stream to log if needed
        }

        // 2. Rebuild backend container
        await foreach (var _ in _processRunner.RunAsync(
            "docker", "compose up -d --build --force-recreate backend", _repoPath, ct))
        {
            // Stream to log if needed
        }

        return Result.Ok();
    }

    public async Task<BackendStatus> GetStatusAsync(CancellationToken ct)
    {
        try
        {
            var psLines = new List<string>();
            await foreach (var line in _processRunner.RunAsync(
                "docker", "compose ps -q backend", _repoPath, ct))
            {
                psLines.Add(line);
            }

            var isRunning = psLines.Any(l => !string.IsNullOrWhiteSpace(l));
            return isRunning ? BackendStatus.Running : BackendStatus.Stopped;
        }
        catch
        {
            return BackendStatus.Error;
        }
    }
}

public enum BackendStatus
{
    Stopped,
    Running,
    Updating,
    Error
}
