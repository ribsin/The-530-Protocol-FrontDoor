// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M62-FrontDoor-Enhancement
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Installation Detector
 * *************************************************************************** */

using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Detects existing Five30 installation and health status.
/// Used on FrontDoor startup to determine if Setup Wizard or Live Monitor should be shown.
/// </summary>
public sealed class Service_InstallationDetector
{
    // Known installation paths (ordered by likelihood)
    private static readonly string[] KnownPaths = new[]
    {
        "/home/ribsin/The-530-Protocol",
        "/home/ribsin/project530",
        "/opt/project530",
        "/root/The-530-Protocol"
    };

    public async Task<InstallationResult> DetectAsync(CancellationToken ct = default)
    {
        var result = new InstallationResult();

        // Find existing installation
        result.RepoPath = await FindInstallationPathAsync();
        if (string.IsNullOrEmpty(result.RepoPath))
        {
            result.IsInstalled = false;
            result.StatusMessage = "No existing installation found.";
            return result;
        }

        result.IsInstalled = true;
        result.StatusMessage = $"Found installation at: {result.RepoPath}";

        // Check if containers are running
        result.ContainersRunning = await CheckContainersAsync(result.RepoPath, ct);
        
        // Check backend health if containers are running
        if (result.ContainersRunning)
        {
            result.IsHealthy = await CheckBackendHealthAsync(ct);
            result.ApiUrl = "http://localhost:5000";
        }
        else
        {
            result.IsHealthy = false;
            result.StatusMessage = "Factory is not running.";
        }

        return result;
    }

    private async Task<string?> FindInstallationPathAsync()
    {
        // Check environment variable first
        var envPath = Environment.GetEnvironmentVariable("FIVE30_REPO_PATH");
#pragma warning disable RS0030
        if (!string.IsNullOrEmpty(envPath) && File.Exists(Path.Combine(envPath, "docker-compose.yml")))
#pragma warning restore RS0030
        {
            return envPath;
        }

        // Check known paths
        foreach (var path in KnownPaths)
        {
            var composeFile = Path.Combine(path, "docker-compose.yml");
#pragma warning disable RS0030
            if (File.Exists(composeFile))
#pragma warning restore RS0030
            {
                return path;
            }
        }

        // Try to find via docker
        var dockerPath = await FindViaDockerAsync();
        return dockerPath;
    }

    private async Task<string?> FindViaDockerAsync()
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/c docker ps --format \"{{.Mounts}}\"" : "-c \"docker ps --format '{{.Mounts}}' 2>/dev/null | grep -o '/[^:]*The-530-Protocol' | head -1\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            if (process == null) return null;

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Parse docker mount paths
            var match = Regex.Match(output, @"([/\w-]+The-530-Protocol)");
            if (match.Success)
            {
                var path = match.Groups[1].Value;
#pragma warning disable RS0030
                if (File.Exists(Path.Combine(path, "docker-compose.yml")))
#pragma warning restore RS0030
                {
                    return path;
                }
            }
        }
        catch
        {
            // Ignore errors
        }

        return null;
    }

    private async Task<bool> CheckContainersAsync(string repoPath, CancellationToken ct)
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "/c docker ps --filter \"name=530\" -q" : $"-c \"cd '{repoPath}' && docker compose ps -q 2>/dev/null || docker ps --filter 'name=530' -q\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            if (process == null) return false;

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            await process.WaitForExitAsync(ct);

            return !string.IsNullOrWhiteSpace(output) && output.Trim().Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckBackendHealthAsync(CancellationToken ct)
    {
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var response = await client.GetAsync("http://localhost:5000/api/health", ct);
            
            if (!response.IsSuccessStatusCode) return false;

            var json = await response.Content.ReadAsStringAsync(ct);
            var health = JsonSerializer.Deserialize<HealthResponse>(json);

            // Consider healthy if Redis is healthy (database might be degraded)
            return health?.Redis?.Status == "healthy";
        }
        catch
        {
            return false;
        }
    }
}

public sealed class InstallationResult
{
    public bool IsInstalled { get; set; }
    public bool ContainersRunning { get; set; }
    public bool IsHealthy { get; set; }
    public string? RepoPath { get; set; }
    public string? ApiUrl { get; set; }
    public string? StatusMessage { get; set; }
}

internal sealed class HealthResponse
{
    public string? Status { get; set; }
    public RedisHealth? Redis { get; set; }
}

internal sealed class RedisHealth
{
    public string? Status { get; set; }
}