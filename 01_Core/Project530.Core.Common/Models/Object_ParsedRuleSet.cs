// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Opus-4.6
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * ************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>
/// Represents a set of rules extracted from a game-specific rule book definition.
/// </summary>
public sealed class ParsedRuleSet
{
    /// <summary>
    /// The game context this rule set applies to (e.g., "SpaceEngineers", "VintageStory").
    /// </summary>
    public string GameContext { get; init; } = string.Empty;

    /// <summary>
    /// Individual rules extracted from the rule book content.
    /// </summary>
    public List<ParsedRule> Rules { get; init; } = new();

    /// <summary>
    /// Metadata tags discovered during parsing.
    /// </summary>
    public List<string> Tags { get; init; } = new();
}

/// <summary>
/// A single rule extracted from a rule book definition.
/// </summary>
public sealed class ParsedRule
{
    /// <summary>
    /// Unique key identifying this rule (e.g., "max_thrust", "block_limit").
    /// </summary>
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Human-readable description of the constraint.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Expected value or threshold for the constraint.
    /// </summary>
    public string ExpectedValue { get; init; } = string.Empty;

    /// <summary>
    /// Severity level if this rule is violated (Info, Warning, Error).
    /// </summary>
    public string Severity { get; init; } = "Warning";
}
