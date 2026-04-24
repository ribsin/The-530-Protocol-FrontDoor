// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Service_FactoryInstaller
 * *************************************************************************** */

using System.Runtime.InteropServices;
using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Orchestrates the full 7-step factory installation:
/// dependency check → git auth → .env.local gen → clone → docker compose up → health poll.
/// </summary>
public sealed class Service_FactoryInstaller : I_FactoryInstaller
{
    private const string RepoUrl = "https://github.com/ribsin/The-530-Protocol.git";
    private const string Branch = "main";
    private const string BackendHealthUrl = "http://localhost:5000";

    private readonly I_ProcessRunner _runner;
    private readonly I_BackendHealthPoller _poller;
    private readonly I_GitCredentialVault _vault;

    public Service_FactoryInstaller(
        I_ProcessRunner runner,
        I_BackendHealthPoller poller,
        I_GitCredentialVault vault)
    {
        _runner = runner;
        _poller = poller;
        _vault = vault;
    }

    public async Task<Result<InstallationReport>> InstallAsync(
        GitCredentials credentials,
        VaultKeys vaultKeys,
        string targetDirectory,
        IProgress<InstallProgress> progress,
        CancellationToken ct)
    {
        // Step 1: Dependency check
        progress.Report(new InstallProgress(InstallStep.DependencyCheck, "Checking dependencies…", 10));
        var depResult = await CheckDependenciesAsync();
        if (!depResult.IsSuccess)
            return Result<InstallationReport>.Fail(depResult.ErrorMessage!);

        // Step 2: Find or determine repo path
        // M62: Check multiple possible installation locations
        var repoPath = await FindRepoPathAsync(targetDirectory);
        
        // Step 3: Git clone if not exists
        if (!Directory.Exists(repoPath))
        {
            progress.Report(new InstallProgress(InstallStep.Clone, $"Cloning {Branch} branch…", 35));
            var cloneResult = await CloneRepositoryAsync(credentials, targetDirectory, progress, ct);
            if (!cloneResult.IsSuccess)
                return Result<InstallationReport>.Fail(cloneResult.ErrorMessage!);
        }
        else
        {
            progress.Report(new InstallProgress(InstallStep.Clone, $"Using existing installation at {repoPath}…", 35));
        }

        // Step 4: Write .env.local (in correct location)
        progress.Report(new InstallProgress(InstallStep.VaultKeys, "Generating .env.local…", 50));
        var envResult = await GenerateEnvLocalAsync(repoPath, vaultKeys);
        if (!envResult.IsSuccess)
            return Result<InstallationReport>.Fail(envResult.ErrorMessage!);

        // Step 5: Docker Compose up (if not running)
        var containersRunning = await CheckContainersAsync(repoPath, ct);
        if (!containersRunning)
        {
            progress.Report(new InstallProgress(InstallStep.DockerUp, "Starting Docker Compose stack…", 65));
            var dockerResult = await DockerComposeUpAsync(repoPath, progress, ct);
            if (!dockerResult.IsSuccess)
                return Result<InstallationReport>.Fail(dockerResult.ErrorMessage!);
        }
        else
        {
            progress.Report(new InstallProgress(InstallStep.DockerUp, "Containers already running…", 65));
            // Restart to pick up new .env.local
            await DockerComposeRestartAsync(repoPath, progress, ct);
        }

        // Step 6: Health poll
        progress.Report(new InstallProgress(InstallStep.HealthCheck, "Waiting for factory to come online…", 80));
        var healthResult = await WaitForHealthyAsync(progress, ct);
        if (!healthResult.IsSuccess)
            return Result<InstallationReport>.Fail(healthResult.ErrorMessage!);

        // Store credentials on success
        var encrypted = ((Service_GitCredentialVault)_vault).EncryptPassword(credentials.Password);
        await _vault.StoreAsync(new GitCredentialEntry(credentials.Username, encrypted, DateTimeOffset.UtcNow, IsDefault: true));
        credentials.Dispose();

        progress.Report(new InstallProgress(InstallStep.HealthCheck, "Factory is online.", 100));
        return Result<InstallationReport>.Ok(new InstallationReport(repoPath, BackendHealthUrl, DateTimeOffset.UtcNow));
    }
    
    private Task<string> FindRepoPathAsync(string targetDirectory)
    {
        // M62: Check known installation paths first
        var knownPaths = new[]
        {
            "/home/ribsin/The-530-Protocol",
            "/opt/project530/The-530-Protocol"
        };
        
        foreach (var path in knownPaths)
        {
#pragma warning disable RS0030
            if (File.Exists(Path.Combine(path, "docker-compose.yml")))
#pragma warning restore RS0030
            {
                return Task.FromResult(path);
            }
        }
        
        // Default to target directory path
        return Task.FromResult(Path.Combine(targetDirectory, "The-530-Protocol"));
    }

    private static async Task<Result<bool>> CheckDependenciesAsync()
    {
        var required = new[] { "git", "docker" };
        foreach (var tool in required)
        {
            var check = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? $"where {tool}"
                : $"which {tool}";
            using var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh",
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"/c where {tool}" : $"-c \"which {tool}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (p == null) return Result<bool>.Fail($"Could not check for '{tool}'.");
            await p.WaitForExitAsync();
            if (p.ExitCode != 0)
                return Result<bool>.Fail($"Required tool not found: '{tool}'. Please install it and retry.");
        }
        return Result<bool>.Ok(true);
    }

    private async Task<Result<bool>> CloneRepositoryAsync(
        GitCredentials credentials,
        string targetDirectory,
        IProgress<InstallProgress> progress,
        CancellationToken ct)
    {
        // M71: Ensure parent directory exists before git clone uses it as CWD.
        // ENOENT crash fix: /home/ribsin/project530 didn't exist when git tried to use it.
#pragma warning disable RS0030
        Directory.CreateDirectory(targetDirectory);
#pragma warning restore RS0030
        var authedUrl = RepoUrl.Replace("https://", $"https://{credentials.Username}:{credentials.Password}@");
        var args = $"clone --branch {Branch} --depth 1 {authedUrl} The-530-Protocol";
        await foreach (var line in _runner.RunAsync("git", args, targetDirectory, ct))
        {
            progress.Report(new InstallProgress(InstallStep.Clone, line, 50));
        }
#pragma warning disable RS0030
        var cloneCheck = File.Exists(Path.Combine(targetDirectory, "The-530-Protocol", "docker-compose.yml"));
#pragma warning restore RS0030
        return cloneCheck
            ? Result<bool>.Ok(true)
            : Result<bool>.Fail("Clone completed but docker-compose.yml was not found in the target directory.");
    }

    private async Task<Result<bool>> DockerComposeUpAsync(
        string repoPath,
        IProgress<InstallProgress> progress,
        CancellationToken ct)
    {
        await foreach (var line in _runner.RunAsync("docker", "compose up -d --build", repoPath, ct))
        {
            progress.Report(new InstallProgress(InstallStep.DockerUp, line, 70));
        }
        return Result<bool>.Ok(true);
    }
    
    private async Task<Result<bool>> DockerComposeRestartAsync(
        string repoPath,
        IProgress<InstallProgress> progress,
        CancellationToken ct)
    {
        // Restart backend to pick up new .env.local
        await foreach (var line in _runner.RunAsync("docker", "compose restart backend", repoPath, ct))
        {
            progress.Report(new InstallProgress(InstallStep.DockerUp, line, 75));
        }
        return Result<bool>.Ok(true);
    }
    
    private async Task<bool> CheckContainersAsync(string repoPath, CancellationToken ct)
    {
        try
        {
            var checkDone = false;
            await foreach (var _ in _runner.RunAsync("docker", "compose ps -q", repoPath, ct))
            {
                checkDone = true;
            }
            return checkDone;
        }
        catch
        {
            return false;
        }
    }

    private async Task<Result<bool>> WaitForHealthyAsync(
        IProgress<InstallProgress> progress,
        CancellationToken ct)
    {
        var baseUri = new Uri(BackendHealthUrl);
        await foreach (var status in _poller.PollAsync(baseUri, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(120), ct))
        {
            progress.Report(new InstallProgress(InstallStep.HealthCheck, $"Backend status: {status}", 85));
            if (status == "healthy") return Result<bool>.Ok(true);
            if (status == "timeout") return Result<bool>.Fail("Backend did not become healthy within 120 seconds.");
        }
        return Result<bool>.Fail("Health check ended unexpectedly.");
    }

    private static Task<Result<bool>> GenerateEnvLocalAsync(string repoPath, VaultKeys keys)
    {
#pragma warning disable RS0030
        if (!Directory.Exists(repoPath))
        {
#pragma warning restore RS0030
            return Task.FromResult(Result<bool>.Ok(true));
        }
        
        var content = string.Join(Environment.NewLine,
            "# [530-PROVENANCE]",
            "# PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50",
            "# PROVENANCE: FRONT-DOOR-AUTO-GENERATED",
            "# AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team",
            "# SECURITY_STATUS: STEEL-CHECK-PASSED",
            "# DOCUMENT: Five30 Vault Configuration",
            "# ============================================================================",
            "# Five30 Protocol — Vault Configuration (Auto-Generated by Front Door)",
            "# ============================================================================",
            "",
            "# --- LLM Gateway Keys (Tier 1: Primary) ---",
            "OPENROUTER_API_KEY=" + keys.OpenRouterApiKey,
            "GEMINI_API_KEY=" + keys.GeminiApiKey,
            "GROQ_API_KEY=" + keys.GroqApiKey,
            "",
            "# --- LLM Gateway Keys (Tier 2: Backup / Specialty) ---",
            "# Together.AI — 5M tokens/month free",
            "TOGETHER_API_KEY=" + keys.TogetherApiKey,
            "",
            "# DeepSeek — very cheap, excellent for code",
            "DEEPSEEK_API_KEY=" + keys.DeepSeekApiKey,
            "",
            "# --- System Configuration ---",
            "FIVE30_GATEKEEPER_ENABLED=false",
            "",
            "# --- Database ---",
            $"POSTGRES_PASSWORD={keys.PostgresPassword}"
        );
        
#pragma warning disable RS0030
        File.WriteAllText(Path.Combine(repoPath, ".env.local"), content);
#pragma warning restore RS0030
        return Task.FromResult(Result<bool>.Ok(true));
    }
}
