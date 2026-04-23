// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-23 | SESSION: M49-LiveStats
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — DashboardStatsDto
 * *************************************************************************** */

namespace Project530.Tools.FrontDoor.Models;

/// <summary>Live dashboard statistics from backend.</summary>
public sealed record DashboardStatsDto(
    int UserCount,
    int ProfileCount,
    int ModPackCount,
    int ModCount,
    int GameRequestCount,
    int TesterResultCount,
    int FailedModPackCount,
    int FailedModCount,
    DateTimeOffset LastUpdated
);
