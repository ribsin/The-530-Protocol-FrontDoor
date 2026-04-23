// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-GPT-5.3-Codex | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * *************************************************************************** */

using System;

namespace Project530.Core.Common.Models;

public sealed record Object_ExternalAsset
{
    public required string PublishedFileId { get; init; }
    public required string Title { get; init; }
    public string? ShortDescription { get; init; }
    public string? PreviewUrl { get; init; }
    public string? DetailsUrl { get; init; }
    public int AppId { get; init; }
    public string? CreatorSteamId { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
