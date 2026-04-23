// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

namespace Project530.Core.Common.Models;

/// <summary>
/// Represents a known incompatibility between two mods, scraped from community sources.
/// </summary>
public sealed record Object_ModConflict
{
    public int Id { get; init; }

    /// <summary>Name or ID of the first mod involved in the conflict.</summary>
    public string Mod1Name { get; init; } = string.Empty;

    /// <summary>Name or ID of the second mod involved in the conflict.</summary>
    public string Mod2Name { get; init; } = string.Empty;

    /// <summary>Category of incompatibility (e.g. "NameCollision", "LoadOrder", "ApiConflict").</summary>
    public string ConflictType { get; init; } = string.Empty;

    /// <summary>URL of the community source that reported this conflict.</summary>
    public string SourceUrl { get; init; } = string.Empty;

    /// <summary>Whether the conflict has been manually confirmed by an operator.</summary>
    public bool IsConfirmed { get; init; }

    public System.DateTime DetectedAt { get; init; } = System.DateTime.UtcNow;
}
