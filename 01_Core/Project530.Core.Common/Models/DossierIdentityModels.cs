// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-GPT-5.2 | AUTHOR_ID: UNKNOWN
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * ************************************************************************** */

using System;

namespace Project530.Core.Common.Models;

public sealed record DiscordIdentityLink(
    string DiscordUserId,
    string? DisplayName,
    string? AvatarUrl
);

public sealed record SteamIdentityLink(
    string SteamId64,
    string? PersonaName,
    string? AvatarUrl
);

public sealed record DualLinkIdentity(
    string ProfileId,
    DiscordIdentityLink? Discord,
    SteamIdentityLink? Steam
);

public sealed record SteamWorkshopItem(
    string PublishedFileId,
    string Title,
    string? PreviewImageUrl,
    DateTimeOffset? LastUpdatedUtc
);

public sealed record DossierIntegrationSettings(
    string ProfileId,
    bool ShowSteamWorkshopItems,
    bool BroadcastDiscordFrequency
);

public sealed record DossierProfile(
    string ProfileId,
    string? ProfileImageUrl
);

public enum ProfileImageSafetyGrade : byte
{
    Unknown = 0,
    Allowed = 1,
    Blocked = 2
}

public sealed record ProfileImageSafetyDecision(
    ProfileImageSafetyGrade Grade,
    string Reason
);
