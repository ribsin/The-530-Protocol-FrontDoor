// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-GPT-5.3-Codex | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * *************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>
/// Classification of work by operational discipline.
/// </summary>
public enum WorkCategory
{
    Requirements,
    Design,
    Implementation,
    Persistence,
    Testing,
    Security,
    Deployment
}

/// <summary>
/// A discrete unit of work assigned during a Five30 work plan session.
/// </summary>
public record WorkOrder
{
    public int DuelId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public WorkCategory WorkCategory { get; init; }
    public string GameContext { get; init; } = string.Empty;

    /// <summary>
    /// Lifecycle status: Pending, InProgress, Resolved.
    /// </summary>
    public string Status { get; init; } = "Pending";

    /// <summary>
    /// Complexity multiplier for scoring (1-5).
    /// </summary>
    public int Complexity { get; init; } = 1;
}

/// <summary>
/// A work order paired with its assigned Duel agents.
/// </summary>
public record WorkPlanItem
{
    public WorkOrder WorkOrder { get; init; } = new();
    public AgentDefinition Primary { get; init; } = new();
    public AgentDefinition Challenger { get; init; } = new();
}

/// <summary>
/// A complete Five30 work plan: Lead Developer + ordered list of duel-assigned work items.
/// </summary>
public record WorkPlan
{
    public string PlanId { get; init; } = string.Empty;
    public bool IsCertified { get; init; }
    public AgentDefinition Lead { get; init; } = new();
    public List<WorkPlanItem> Items { get; init; } = new();
}
