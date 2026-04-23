// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

using System;

namespace Project530.Core.Common.Models;

/// <summary>
/// Immutable audit event record for the Black Box telemetry system.
/// Records agent actions, system events, and operational milestones.
/// </summary>
public sealed record Object_BlackBoxEvent
{
    public int Id { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string EventType { get; init; } = string.Empty;
    public string AgentId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Metadata { get; init; } = string.Empty;
}
