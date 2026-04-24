// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M50-FrontDoor-Fixes
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: AI Agent Config Entry DTO
 * *************************************************************************** */

namespace Project530.Tools.FrontDoor.Models;

/// <summary>Represents a configured AI agent/provider entry in the factory.</summary>
public sealed class Object_AiAgentConfigEntry
{
    public string Provider { get; init; } = string.Empty;
    public string Endpoint { get; init; } = string.Empty;
    public string MaskedKey { get; init; } = string.Empty;
    public string Status { get; init; } = "Unknown";

    /// <summary>Parses a model identifier string to extract the provider name.</summary>
    public static string ExtractProvider(string modelId)
    {
        if (string.IsNullOrEmpty(modelId)) return "Unknown";

        // Handle "provider/model-name" format
        if (modelId.Contains('/'))
            return modelId.Split('/')[0];

        // Handle "openrouter:provider/model" format
        if (modelId.Contains(':'))
            return modelId.Split(':')[1].Split('/')[0];

        // Fallback: first word before first non-letter character
        var idx = modelId.IndexOfAny(new[] { '-', '_', ':', '/' });
        return idx > 0 ? modelId[..idx] : modelId;
    }

    /// <summary>Returns a masked version of the API key for display.</summary>
    public static string MaskKey(string key)
    {
        if (string.IsNullOrEmpty(key) || key.Length < 4)
            return "••••••••";
        return new string('•', Math.Min(key.Length, 12));
    }
}