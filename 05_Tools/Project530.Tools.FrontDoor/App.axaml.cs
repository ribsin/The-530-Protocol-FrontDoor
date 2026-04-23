// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — App.axaml.cs
 * *************************************************************************** */

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

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services = BuildServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<ViewModel_SetupWizard>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServices()
    {
        var vault = new Service_GitCredentialVault();
        var runner = new Service_ProcessRunner();
        var poller = new Service_BackendHealthPoller();
        var installer = new Service_FactoryInstaller(runner, poller, vault);

        var services = new ServiceCollection();
        services.AddSingleton<I_GitCredentialVault>(vault);
        services.AddSingleton<Service_GitCredentialVault>(vault);
        services.AddSingleton<I_ProcessRunner>(runner);
        services.AddSingleton<I_BackendHealthPoller>(poller);
        services.AddSingleton<I_FactoryInstaller>(installer);
        services.AddTransient<ViewModel_SetupWizard>();
        services.AddTransient<ViewModel_LiveMonitor>(sp =>
            new ViewModel_LiveMonitor("http://localhost:5000"));
        services.AddTransient<ViewModel_Settings>(sp =>
            new ViewModel_Settings(vault, vault));
        return services.BuildServiceProvider();
    }
}
