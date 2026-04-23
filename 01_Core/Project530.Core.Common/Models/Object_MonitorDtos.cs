// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * *************************************************************************** */

using System;
using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>API response DTO representing an agent's operational status in the Remote Monitor TUI.</summary>
public record Object_AgentStatusDto(
    int Id,
    string Name,
    string Role,
    string Category,
    string PreferredModel,
    int ComplianceScore,
    int ReputationDebt,
    string WorkStreamStatus,
    DateTime? LastActive);

/// <summary>API response DTO for a work order entry in the Remote Monitor TUI.</summary>
public record Object_WorkOrderDto(string Title, string Status, string WorkCategory);

/// <summary>API response DTO for a paired work plan item in the Remote Monitor TUI.</summary>
public record Object_WorkPlanItemDto(
    Object_WorkOrderDto WorkOrder,
    Object_AgentStatusDto Primary,
    Object_AgentStatusDto Challenger);

/// <summary>API response DTO for a serialized work plan in the Remote Monitor TUI.</summary>
public record Object_WorkPlanDto(
    string PlanId,
    bool IsCertified,
    Object_AgentStatusDto Lead,
    List<Object_WorkPlanItemDto> Items);

/// <summary>API response DTO for factory health status in the Remote Monitor TUI.</summary>
public record Object_HealthDto(string Status, string Message);
