// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-GPT-5.3-Codex | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * *************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

/// <summary>
/// Provides the canonical initial roster templates used for first-time database seeding.
/// </summary>
public static class AgentRosterSeedData
{
    public static IReadOnlyList<AgentDefinition> InitialAgentRoster { get; } =
    [
        Create(1, "Foreman", "Lead Supervisor & Git Auditor", "Leadership", "google/gemini-2.5-flash:free"),
        Create(2, "Ticker Object_Agent", "Chain of Custody Specialist", "Leadership", "google/gemini-2.5-flash:free"),
        Create(3, "Design Bay", "Visual Prompt Engineer", "Leadership", "google/gemini-2.5-flash:free"),
        Create(4, "Domain Architect", "Intent Clarifier — Domain Specialist", "Development", "meta-llama/llama-3.3-70b-instruct:free"),
        Create(5, "Service Builder", "Intent Clarifier — Service Orchestrator", "Development", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(6, "Repository Guardian", "Game-Context Scripter (Space Engineers)", "Development", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(7, "Interface Weaver", "Game-Context Scripter (Space Engineers)", "Development", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(8, "Event Coordinator", "Game-Context Scripter (Vintage Story)", "Development", "google/gemini-2.5-flash:free"),
        Create(9, "Result Formatter", "Game-Context Scripter (Vintage Story)", "Development", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(10, "Factory Supervisor", "Game-Context Scripter (Vintage Story)", "Development", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(11, "Steel-Check Enforcer", "Security Infrastructure & Integrity Guardian", "Security & Compliance", "meta-llama/llama-3.3-70b-instruct:free"),
        Create(12, "Guardian Sentinel", "Runtime Security Monitor", "Security & Compliance", "meta-llama/llama-3.3-70b-instruct:free"),
        Create(13, "Compliance Scribe", "Chain of Custody Documentation", "Security & Compliance", "meta-llama/llama-3.3-70b-instruct:free"),
        Create(14, "Analyzer Sage", "Roslyn Static Analysis", "Security & Compliance", "meta-llama/llama-3.3-70b-instruct:free"),
        Create(15, "Privacy Protector", "Data Protection Officer", "Security & Compliance", "meta-llama/llama-3.3-70b-instruct:free"),
        Create(16, "Broom Tester", "Surface-Level Testing Specialist", "Testing & QA", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(17, "Root Validator", "Deep Integration Testing", "Testing & QA", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(18, "Load Rater", "Performance Testing Specialist", "Testing & QA", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(19, "Scenario Builder", "Test Case Designer", "Testing & QA", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(20, "Quality Champion", "Quality Assurance Lead", "Testing & QA", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(21, "VRAGE Connector", "Game-Context Scripter (Space Engineers)", "Integration", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(22, "Browser Liaison", "Headless Asset Scraper", "Integration", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(23, "Persistence Manager", "Headless Asset Scraper — Storage", "Integration", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(24, "API Gateway", "Headless Asset Scraper — API", "Integration", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(25, "Message Broker", "Headless Asset Scraper — Messaging", "Integration", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(26, "Phosphor Stylist", "Visual Prompt Engineer — Theme Specialist", "UI & Design", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(27, "Component Crafter", "Visual Prompt Engineer — Component Architect", "UI & Design", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(28, "Asset Designer", "Visual Prompt Engineer — Asset Specialist", "UI & Design", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(29, "Documentation Poet", "Technical Writing Specialist", "Documentation", "qwen/qwen-2.5-coder-32b-instruct:free"),
        Create(30, "Communication Facilitator", "Cross-Object_Agent Communication Coordinator", "Documentation", "google/gemini-2.5-flash:free"),
        Create(31, "Texture Engineer", "Image Generation — Texture and Material Specialist", "Image Generation", "google/gemini-2.5-flash:free"),
        Create(32, "Environment Illustrator", "Image Generation — Scene and Environment Compositor", "Image Generation", "google/gemini-2.5-flash:free"),
        Create(33, "Prop Modeler", "Image Generation — Props and Component Visualizer", "Image Generation", "google/gemini-2.5-flash:free")
    ];

    private static AgentDefinition Create(int id, string name, string role, string category, string preferredModel)
    {
        return new AgentDefinition
        {
            Id = id,
            Name = name,
            Role = role,
            Category = category,
            PreferredModel = preferredModel,
            ComplianceScore = 100,
            SystemPrompt = "You are " + name + ". " + role + "."
        };
    }
}
