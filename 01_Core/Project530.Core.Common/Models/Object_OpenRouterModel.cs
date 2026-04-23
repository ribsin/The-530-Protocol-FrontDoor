// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Opus-4.6 | AUTHOR_ID: 530-Protocol-Team
 * LAST_MODIFIED: 2026-04-07
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * *************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>
/// Represents a single AI model entry from the OpenRouter /models manifest.
/// </summary>
public sealed class Object_OpenRouterModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ContextLength { get; set; }
    public OpenRouterPricing Pricing { get; set; } = new OpenRouterPricing();
    public string Architecture { get; set; } = string.Empty;
    public bool IsFree { get; set; }
}

/// <summary>
/// Pricing info returned by OpenRouter for a model.
/// </summary>
public sealed class OpenRouterPricing
{
    public string Prompt { get; set; } = "0";
    public string Completion { get; set; } = "0";
}

/// <summary>
/// The full manifest response from the OpenRouter /models endpoint.
/// </summary>
public sealed class Object_OpenRouterManifest
{
    public List<Object_OpenRouterModel> Models { get; set; } = new List<Object_OpenRouterModel>();
    public string RetrievedAt { get; set; } = string.Empty;
    public int TotalCount { get; set; }
}

/// <summary>
/// Defines the pricing tier filter for OpenRouter model selection.
/// Free is the default; paid tiers require explicit opt-in from Mission Control.
/// </summary>
public enum OpenRouterTier
{
    Free = 0,
    Budget = 1,
    Mid = 2,
    Premium = 3
}
