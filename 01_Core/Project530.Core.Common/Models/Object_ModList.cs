// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-v1 | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

public sealed record Object_ModList
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string OperatorId { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public string ShareCode { get; init; } = string.Empty;
    public System.DateTime CreatedAt { get; init; } = System.DateTime.UtcNow;
    /// <summary>Optional: the Object_Agent ID that generated this modlist (null = operator-created).</summary>
    public int? GeneratedByAgentId { get; init; }
    public List<Object_ModListItem> Items { get; init; } = new();
}
