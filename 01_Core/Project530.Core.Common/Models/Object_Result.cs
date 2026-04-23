// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

namespace Project530.Core.Common.Models;

/// <summary>
/// Represents a structured error with a code and human-readable description.
/// Used as the error payload inside <see cref="Result{T}"/>.
/// </summary>
public sealed record Error(string Code, string Description)
{
    /// <summary>Creates a generic error from a plain message string.</summary>
    public static Error From(string description) => new("ERR", description);
}
