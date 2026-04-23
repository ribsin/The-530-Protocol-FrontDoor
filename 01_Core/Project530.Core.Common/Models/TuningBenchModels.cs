// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-GPT-5.2 | AUTHOR_ID: UNKNOWN
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * ************************************************************************** */

namespace Project530.Core.Common.Models;

public enum PerformanceGrade : byte
{
    Optimal = 1,
    Nominal = 2,
    Strained = 3
}

/// <summary>
/// Front-end safe summary of a crate (no telemetry payload).
/// </summary>
public sealed record BenchCrateSummary(string CrateID, string Origin);

/// <summary>
/// Editable "Coupon" parameters for a Reactor block.
/// Note: excludes memory/alloc/telemetry. PerformanceRating is grade-only (no raw sim-speed).
/// </summary>
public sealed record ReactorCoupon(
    int EnergyYield,
    int FuelUsage,
    int CoolingFlow,
    int ArmorRating,
    PerformanceGrade PerformanceRating
);
