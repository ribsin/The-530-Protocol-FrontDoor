// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M63-FrontDoor-Fix
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — App.axaml.cs
 * *************************************************************************** */

using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Project530.Tools.FrontDoor.Services;
using Project530.Tools.FrontDoor.ViewModels;
using Project530.Tools.FrontDoor.Views;

namespace Project530.Tools.FrontDoor;

public sealed class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static Service_StartupLogger Logger { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        // M63: Initialize logger first for diagnostics
        Logger = new Service_StartupLogger();
        Logger.Info("=== Five30 FrontDoor Starting ===");
        Logger.Info($"Platform: {RuntimeInformation.OSDescription}");
        Logger.Info($"Runtime: {RuntimeInformation.FrameworkDescription}");
        
        Services = BuildServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // M63: Async detection with timeout to prevent UI freeze
            var detector = Services.GetRequiredService<Service_InstallationDetector>();
            Logger.Info("Starting installation detection...");
            
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var result = await Task.Run(() => detector.DetectAsync(cts.Token), cts.Token);
                
                Logger.Info($"Detection complete: IsInstalled={result.IsInstalled}, IsHealthy={result.IsHealthy}, Path={result.RepoPath}");
                
                if (result.IsInstalled && result.IsHealthy)
                {
                    // Factory is running - show Live Monitor
                    Logger.Info("Showing Live Monitor (factory healthy)");
                    var liveMonitor = new MainWindow
                    {
                        DataContext = Services.GetRequiredService<ViewModel_LiveMonitor>()
                    };
                    Logger.Info("Live Monitor window created");
                    desktop.MainWindow = liveMonitor;
                    Logger.Info("Live Monitor window assigned to desktop");
                }
                else
                {
                    // Not installed or needs config - show Setup Wizard
                    Logger.Info("Showing Setup Wizard (factory not ready)");
                    Logger.Info("Resolving ViewModel_SetupWizard from DI...");
                    var wizardVm = Services.GetRequiredService<ViewModel_SetupWizard>();
                    Logger.Info($"ViewModel resolved: {wizardVm?.GetType().Name ?? "null"}");
                    Logger.Info("Creating MainWindow...");
                    var mainWindow = new MainWindow
                    {
                        DataContext = wizardVm
                    };
                    Logger.Info("MainWindow created, assigning to desktop...");
                    desktop.MainWindow = mainWindow;
                    Logger.Info("MainWindow assigned to desktop");
                    
                    // M67: Force window to show and activate
                    Logger.Info("Showing window...");
                    mainWindow.Show();
                    Logger.Info("Activating window...");
                    mainWindow.Activate();
                    Logger.Info($"Window state: {mainWindow.WindowState}");
                    Logger.Info($"Window position: {mainWindow.Position}");
                    
                    // Fallback: ensure window is visible
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        Logger.Info("Window was minimized — restoring...");
                        mainWindow.WindowState = WindowState.Normal;
                    }
                    
                    // Ensure window is on screen
                    mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    Logger.Info("Set window startup location to CenterScreen");
                }
            }
            catch (OperationCanceledException)
            {
                Logger.Warn("Detection timed out after 5 seconds - showing Setup Wizard");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<ViewModel_SetupWizard>()
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Detection failed", ex);
                Logger.Info("Showing Setup Wizard (detection error)");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Services.GetRequiredService<ViewModel_SetupWizard>()
                };
            }
        }

        Logger.Info($"=== FrontDoor Ready | Log: {Logger.GetLogPath()} ===");
        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var vault = new Service_GitCredentialVault();
        var runner = new Service_ProcessRunner();
        var poller = new Service_BackendHealthPoller();
        var installer = new Service_FactoryInstaller(runner, poller, vault);
        var detector = new Service_InstallationDetector();

        var services = new ServiceCollection();
        services.AddSingleton<I_GitCredentialVault>(vault);
        services.AddSingleton<Service_GitCredentialVault>(vault);
        services.AddSingleton<I_ProcessRunner>(runner);
        services.AddSingleton<I_BackendHealthPoller>(poller);
        services.AddSingleton<I_FactoryInstaller>(installer);
        services.AddSingleton<Service_InstallationDetector>(detector);
        services.AddTransient<ViewModel_SetupWizard>();
        services.AddTransient<ViewModel_LiveMonitor>(sp =>
            new ViewModel_LiveMonitor("http://localhost:5000"));
        return services.BuildServiceProvider();
    }
}