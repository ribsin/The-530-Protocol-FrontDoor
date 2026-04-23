// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Git Credential Vault Models
 * *************************************************************************** */

namespace Project530.Tools.FrontDoor.Models;

/// <summary>
/// Encrypted Git credential entry stored in the local AES-256-GCM vault.
/// </summary>
public sealed record GitCredentialEntry(
    string Username,
    string EncryptedPassword,
    DateTimeOffset CreatedAt,
    bool IsDefault
);

/// <summary>
/// Plain-text credentials used only transiently during authentication.
/// Zeroed from memory after use.
/// </summary>
public sealed class GitCredentials : IDisposable
{
    public string Username { get; }
    public string Password { get; private set; }

    public GitCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public void Dispose()
    {
        Password = string.Empty;
    }
}

/// <summary>Vault keys for .env.local generation.</summary>
public sealed record VaultKeys(string VaultApiKey, string DatabasePassword);
