// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.5 | AUTHOR_ID: Five30-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * *************************************************************************** */

using System;
using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>
/// Represents a Five30 Protocol agent definition with configuration and routing metadata.
/// </summary>
public record AgentDefinition
{
    /// <summary>
    /// Unique identifier for the agent (1-33).
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Display name of the agent (e.g., "Foreman", "Design Bay").
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Object_Agent's primary role or specialty area.
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// System prompt template for this agent's specific behavior.
    /// </summary>
    public string SystemPrompt { get; init; } = string.Empty;

    /// <summary>
    /// Preferred AI model for this agent (e.g., "Claude", "GPT-4", "Gemini").
    /// </summary>
    public string PreferredModel { get; init; } = string.Empty;

    /// <summary>
    /// Functional category for task assignment (e.g., "Security", "Development").
    /// </summary>
    public string Category { get; init; } = "General";

    /// <summary>
    /// Automated quality score based on past performance (0-100).
    /// </summary>
    public int ComplianceScore { get; init; } = 100;

    /// <summary>
    /// Timestamp of the last Foreman inspection.
    /// </summary>
    public DateTime? LastAudit { get; init; }

    /// <summary>
    /// Count of automated corrections applied to this agent's output.
    /// </summary>
    public int CorrectionsApplied { get; init; } = 0;

    /// <summary>
    /// Current operational status (e.g., "Nominal", "Penance Required", "Under Review").
    /// </summary>
    public string WorkStreamStatus { get; init; } = "Nominal";

    /// <summary>
    /// Number of strikes for non-compliance, per Monastery Rule (Max 3).
    /// </summary>
    public int AccountabilityStrikes { get; init; } = 0;

    /// <summary>
    /// Accumulated reputation debt from failed audits. Must be worked off.
    /// </summary>
    public int ReputationDebt { get; init; } = 0;

    /// <summary>
    /// Performance score earned in Lead Developer rotation (0-100).
    /// </summary>
    public int LeadPerformanceScore { get; init; } = 100;

    /// <summary>
    /// Timestamp of last recorded activity for this agent.
    /// </summary>
    public DateTime? LastActive { get; init; }

    /// <summary>
    /// Earned specialization tags (e.g. "Implementation", "Security") gained from resolved Duels.
    /// </summary>
    public List<string> EarnedTags { get; init; } = new();
}
