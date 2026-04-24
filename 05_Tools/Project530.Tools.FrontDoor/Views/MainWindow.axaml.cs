// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — MainWindow.axaml.cs
 * *************************************************************************** */

using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Project530.Tools.FrontDoor.ViewModels;

namespace Project530.Tools.FrontDoor.Views;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        App.Logger?.Info("MainWindow: Starting InitializeComponent...");
        InitializeComponent();
        App.Logger?.Info("MainWindow: InitializeComponent complete");
        
        App.Logger?.Info("MainWindow: Resolving ViewModel_SetupWizard...");
        var wizard = App.Services.GetRequiredService<ViewModel_SetupWizard>();
        App.Logger?.Info("MainWindow: ViewModel resolved, attaching event handler...");
        wizard.InstallationComplete += OnInstallationComplete;
        WizardView.DataContext = wizard;
        App.Logger?.Info("MainWindow: WizardView DataContext set");
    }

    private void OnInstallationComplete(object? sender, EventArgs e)
    {
        var monitor = App.Services.GetRequiredService<ViewModel_LiveMonitor>();
        MonitorView.DataContext = monitor;
        WizardView.IsVisible = false;
        MonitorView.IsVisible = true;
        _ = monitor.ConnectCommand.ExecuteAsync(null);
    }
}
