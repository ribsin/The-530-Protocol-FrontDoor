// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED | AGENT_ID: GitHub-Copilot-Claude-Haiku-4.5
 * SECURITY_STATUS: STEEL-CHECK-COMPLIANT
 * ************************************************************************** */

using System;
using System.Collections.Generic;

namespace Project530.Core.Common.Models;

public sealed record ValidationReport
{
    public string AssetId { get; init; } = "UNIDENTIFIED";
    public bool IsCompliant { get; init; } = false;
    public List<string> ComplianceIssues { get; init; } = new();
    public List<string> ComplianceNotes { get; init; } = new();
    public DateTime ValidatedAt { get; init; } = DateTime.UtcNow;
    public string RuleBookName { get; init; } = "Unknown";

    public string Summary
    {
        get => IsCompliant ? "✓ Asset is compliant" : "✗ Asset has compliance issues";
    }
}
