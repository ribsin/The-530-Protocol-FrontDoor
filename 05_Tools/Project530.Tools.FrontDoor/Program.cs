// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Program.cs
 * *************************************************************************** */

using Avalonia;
using Avalonia.ReactiveUI;
using Project530.Tools.FrontDoor;

AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .LogToTrace()
    .UseReactiveUI()
    .StartWithClassicDesktopLifetime(args);
