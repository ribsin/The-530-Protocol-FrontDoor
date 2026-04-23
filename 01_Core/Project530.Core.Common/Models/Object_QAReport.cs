// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>
/// Pass/fail verdict produced by the 10-step modpack QA pipeline.
/// Aggregates critical issues and warnings into a compliance score.
/// </summary>
public sealed record Object_QAReport
{
    public int Id { get; init; }

    /// <summary>The modpack this report was generated for.</summary>
    public int ModListId { get; init; }

    /// <summary>Overall pass/fail verdict. False if any game-breaking critical issues are present.</summary>
    public bool IsPass { get; init; }

    /// <summary>Aggregate compliance score from 0 (fail) to 100 (perfect).</summary>
    public int ComplianceScore { get; init; }

    /// <summary>Game-breaking issues that force IsPass = false (missing deps, crashes, etc.).</summary>
    public List<string> CriticalIssues { get; init; } = new List<string>();

    /// <summary>Non-blocking issues that reduce ComplianceScore but do not fail the report.</summary>
    public List<string> Warnings { get; init; } = new List<string>();

    public System.DateTime GeneratedAt { get; init; } = System.DateTime.UtcNow;
}
