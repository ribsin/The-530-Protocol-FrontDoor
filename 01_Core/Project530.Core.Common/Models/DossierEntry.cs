// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: HUMAN-REFINED
 * AGENT_ID: GitHub-Copilot-GPT-5.4 | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * ************************************************************************** */

using System;
using System.Collections.Generic;

namespace Project530.Core.Common.Models;

public sealed record DossierEntry
{
    public Guid ExportID { get; init; } = Guid.NewGuid();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string BlockType { get; init; } = string.Empty;
    public string SourceStamp { get; init; } = string.Empty;
    public string ModSummary { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string DesignSeed { get; init; } = string.Empty;
    public Dictionary<string, object> ModifiedVariables { get; init; } = new();
}
