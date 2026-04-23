// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-v1 | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

namespace Project530.Core.Common.Models;

public sealed record Object_ModListItem
{
    public int Id { get; init; }
    public int ModListId { get; init; }
    public string ExternalAssetId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public System.DateTime AddedAt { get; init; } = System.DateTime.UtcNow;
    public ConflictRisk ConflictRisk { get; init; }
    public string AiReasoning { get; init; } = string.Empty;
    public int HealthScore { get; init; }
}
