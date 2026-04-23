/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Gemini-Architect-v1 | AUTHOR_ID: 530-Protocol-Team
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * ************************************************************************** */

using System.Collections.Generic;

namespace Project530.Core.Common.Models;

public readonly record struct Object_AssetModel
{
    public Object_AssetModel()
    {
        Refinery = new();
        ExportHistory = new();
        DesignSeed = null;
        EnergyDraw = 1.0;
        RefinementRate = 1.0;
        AssemblySpeed = 1.0;
        Efficiency = 1.0;
        ThrustForce = 1.0;
        FuelEfficiency = 1.0;
        SubtypeId = "UNIDENTIFIED_BLOCK";
        BaseClass = "UNIDENTIFIED_BLOCK";
        Origin = "WorkshopNative";
    }

    public RefineryProperties Refinery { get; init; }
    public List<DossierEntry> ExportHistory { get; init; }

    public string? DesignSeed { get; init; }
    public double EnergyDraw { get; init; }
    public double RefinementRate { get; init; }
    public double AssemblySpeed { get; init; }
    public double Efficiency { get; init; }
    public double ThrustForce { get; init; }
    public double FuelEfficiency { get; init; }
    public string SubtypeId { get; init; }
    public string BaseClass { get; init; }
    public string Origin { get; init; }
}
