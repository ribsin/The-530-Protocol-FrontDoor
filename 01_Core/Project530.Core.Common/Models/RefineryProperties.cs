// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: HUMAN-REFINED
 * AGENT_ID: GitHub-Copilot-GPT-5.4 | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * ************************************************************************** */

namespace Project530.Core.Common.Models;

public sealed record RefineryProperties
{
    public double RefinementRate { get; init; } = 1.0;
    public double EnergyDraw { get; init; } = 1.0;
    public double YieldEfficiency { get; init; } = 1.0;
}
