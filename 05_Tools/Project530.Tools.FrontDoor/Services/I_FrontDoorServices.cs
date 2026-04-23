// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Service Interfaces (ISP-compliant)
 * *************************************************************************** */

using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Orchestrates the full factory installation sequence:
/// dependency check → git auth → .env.local gen → clone → docker compose up → health poll.
/// </summary>
public interface I_FactoryInstaller
{
    Task<Result<InstallationReport>> InstallAsync(
        GitCredentials credentials,
        VaultKeys vaultKeys,
        string targetDirectory,
        IProgress<InstallProgress> progress,
        CancellationToken ct);
}

/// <summary>Encrypted local storage for Git credentials.</summary>
public interface I_GitCredentialVault
{
    Task StoreAsync(GitCredentialEntry entry);
    Task<GitCredentialEntry?> GetDefaultAsync();
    Task<IReadOnlyList<GitCredentialEntry>> ListAsync();
    Task RemoveAsync(string username);
}

/// <summary>Polls the backend health endpoint until healthy or timed out.</summary>
public interface I_BackendHealthPoller
{
    IAsyncEnumerable<string> PollAsync(Uri baseUrl, TimeSpan interval, TimeSpan timeout, CancellationToken ct);
}

/// <summary>Runs a shell process and streams its stdout/stderr lines.</summary>
public interface I_ProcessRunner
{
    IAsyncEnumerable<string> RunAsync(string fileName, string arguments, string workingDirectory, CancellationToken ct);
}
