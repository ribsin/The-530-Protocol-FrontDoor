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
/// Stores operator-defined hardware profile and performance threshold values
/// used by the QA Orchestrator's compliance step validation.
/// </summary>
public sealed record Object_ComplianceThresholds
{
    public int Id { get; init; }

    /// <summary>Steam ID of the operator who owns this profile.</summary>
    public string OperatorId { get; init; } = string.Empty;

    /// <summary>Human-readable label for this hardware profile (e.g. "High-End Rig").</summary>
    public string ProfileName { get; init; } = "Default Profile";

    /// <summary>Minimum acceptable frames per second.</summary>
    public int MinFps { get; init; } = 30;

    /// <summary>Maximum acceptable modpack load time in milliseconds.</summary>
    public int MaxLoadTimeMs { get; init; } = 5000;

    /// <summary>Maximum acceptable memory footprint in megabytes.</summary>
    public int MaxMemoryUsageMb { get; init; } = 8192;

    /// <summary>Maximum acceptable server tick lag in milliseconds.</summary>
    public int MaxTickLagMs { get; init; } = 100;

    public System.DateTime CreatedAt { get; init; } = System.DateTime.UtcNow;
    public System.DateTime UpdatedAt { get; init; } = System.DateTime.UtcNow;
}
