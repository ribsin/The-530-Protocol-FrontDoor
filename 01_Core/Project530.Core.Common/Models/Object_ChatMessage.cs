// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-GPT-5.3-Codex | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * *************************************************************************** */

using System;

namespace Project530.Core.Common.Models;

public sealed record Object_ChatMessage
{
    public required string Role { get; init; }
    public required string Content { get; init; }
    public required DateTime TimestampUtc { get; init; }
}
