// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — ViewModel_SetupWizard
 * *************************************************************************** */

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project530.Tools.FrontDoor.Models;
using Project530.Tools.FrontDoor.Services;

namespace Project530.Tools.FrontDoor.ViewModels;

public sealed partial class ViewModel_SetupWizard : ViewModel_Base
{
    private readonly I_FactoryInstaller _installer;

    [ObservableProperty] private int _currentStep = 0;
    [ObservableProperty] private string _statusMessage = "Welcome to the Five30 Front Door.";
    [ObservableProperty] private int _progressPercent = 0;
    [ObservableProperty] private bool _isInstalling = false;
    [ObservableProperty] private bool _hasError = false;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _openRouterApiKey = string.Empty;
    [ObservableProperty] private string _geminiApiKey = string.Empty;
    [ObservableProperty] private string _groqApiKey = string.Empty;
    [ObservableProperty] private string _postgresPassword = string.Empty;
    [ObservableProperty] private string _targetDirectory = DefaultTargetDirectory();

    public ObservableCollection<string> LogLines { get; } = new();

    public event EventHandler? InstallationComplete;

    public ViewModel_SetupWizard(I_FactoryInstaller installer)
    {
        _installer = installer;
    }

    [RelayCommand]
    private void NextStep() => CurrentStep = Math.Min(CurrentStep + 1, (int)InstallStep.HealthCheck);

    [RelayCommand]
    private async Task BeginInstallAsync(CancellationToken ct)
    {
        IsInstalling = true;
        HasError = false;
        LogLines.Clear();

        using var credentials = new GitCredentials(Username, Password);
        var keys = new VaultKeys(OpenRouterApiKey, GeminiApiKey, GroqApiKey, PostgresPassword);
        var progress = new Progress<InstallProgress>(p =>
        {
            CurrentStep = (int)p.Step;
            StatusMessage = p.Message;
            ProgressPercent = p.PercentComplete;
            LogLines.Add($"[{p.Step}] {p.Message}");
        });

        var result = await _installer.InstallAsync(credentials, keys, TargetDirectory, progress, ct);
        IsInstalling = false;

        if (result.IsSuccess)
        {
            StatusMessage = "Factory is online. Launching Live Monitor…";
            InstallationComplete?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            HasError = true;
            StatusMessage = result.ErrorMessage ?? "Installation encountered an issue.";
            LogLines.Add($"[ERROR] {StatusMessage}");
        }
    }

    private static string DefaultTargetDirectory()
    {
        return Environment.OSVersion.Platform == PlatformID.Unix
            ? "$HOME/project530"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "project530");
    }
}
