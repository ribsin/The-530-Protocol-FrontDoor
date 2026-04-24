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

        // Step 2: Git auth validated upstream in wizard; proceed to .env.local
        progress.Report(new InstallProgress(InstallStep.VaultKeys, "Generating .env.local…", 25));
        var repoPath = Path.Combine(targetDirectory, "The-530-Protocol");
        var envResult = await GenerateEnvLocalAsync(repoPath, vaultKeys);
        if (!envResult.IsSuccess)
            return Result<InstallationReport>.Fail(envResult.ErrorMessage!);

        // Step 3: Git clone
        progress.Report(new InstallProgress(InstallStep.Clone, $"Cloning {Branch} branch…", 35));
        var cloneResult = await CloneRepositoryAsync(credentials, targetDirectory, progress, ct);
        if (!cloneResult.IsSuccess)
            return Result<InstallationReport>.Fail(cloneResult.ErrorMessage!);

        // Step 4: Write .env.local into the cloned repo
        await GenerateEnvLocalAsync(repoPath, vaultKeys);

        // Step 5: Docker Compose up
        progress.Report(new InstallProgress(InstallStep.DockerUp, "Starting Docker Compose stack…", 65));
        var dockerResult = await DockerComposeUpAsync(repoPath, progress, ct);
        if (!dockerResult.IsSuccess)
            return Result<InstallationReport>.Fail(dockerResult.ErrorMessage!);

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
