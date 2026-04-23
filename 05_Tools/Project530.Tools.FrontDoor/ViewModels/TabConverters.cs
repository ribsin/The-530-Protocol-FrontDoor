// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-23 | SESSION: M50-TabUI
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Tab Converter Classes
 * *************************************************************************** */

using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Project530.Tools.FrontDoor.ViewModels;

/// <summary>Converters for tab visibility and styling.</summary>
public static class TabConverters
{
    // Active tab index
    public const int StatsTab = 0;
    public const int LiveOpsTab = 1;
    public const int WorkforceTab = 2;
    public const int BrainPoolTab = 3;

    // IsVisible converters
    public static readonly FuncValueConverter<int, bool> IsStats = new(t => t == StatsTab);
    public static readonly FuncValueConverter<int, bool> IsLiveOps = new(t => t == LiveOpsTab);
    public static readonly FuncValueConverter<int, bool> IsWorkforce = new(t => t == WorkforceTab);
    public static readonly FuncValueConverter<int, bool> IsBrainPool = new(t => t == BrainPoolTab);

    // Button background converter (highlight active tab)
    public static readonly FuncValueConverter<int, IBrush> Stats = new(t => 
        t == StatsTab ? new SolidColorBrush(Color.Parse("#2D7DD2")) : Brushes.Transparent);
    public static readonly FuncValueConverter<int, IBrush> LiveOps = new(t => 
        t == LiveOpsTab ? new SolidColorBrush(Color.Parse("#2D7DD2")) : Brushes.Transparent);
    public static readonly FuncValueConverter<int, IBrush> Workforce = new(t => 
        t == WorkforceTab ? new SolidColorBrush(Color.Parse("#2D7DD2")) : Brushes.Transparent);
    public static readonly FuncValueConverter<int, IBrush> BrainPool = new(t => 
        t == BrainPoolTab ? new SolidColorBrush(Color.Parse("#2D7DD2")) : Brushes.Transparent);
}
