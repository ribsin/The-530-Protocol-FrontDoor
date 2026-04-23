// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — ViewModel_Settings
 * *************************************************************************** */

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project530.Tools.FrontDoor.Models;
using Project530.Tools.FrontDoor.Services;

namespace Project530.Tools.FrontDoor.ViewModels;

public sealed partial class ViewModel_Settings : ViewModel_Base
{
    private readonly I_GitCredentialVault _vault;
    private readonly Service_GitCredentialVault _vaultImpl;

    [ObservableProperty] private string _newUsername = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public ObservableCollection<GitCredentialEntry> Credentials { get; } = new();

    public ViewModel_Settings(I_GitCredentialVault vault, Service_GitCredentialVault vaultImpl)
    {
        _vault = vault;
        _vaultImpl = vaultImpl;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Credentials.Clear();
        foreach (var entry in await _vault.ListAsync())
            Credentials.Add(entry);
    }

    [RelayCommand]
    private async Task AddCredentialAsync()
    {
        if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword))
        {
            StatusMessage = "Username and password are both required.";
            return;
        }
        var encrypted = _vaultImpl.EncryptPassword(NewPassword);
        await _vault.StoreAsync(new GitCredentialEntry(NewUsername, encrypted, DateTimeOffset.UtcNow, IsDefault: !Credentials.Any()));
        NewUsername = string.Empty;
        NewPassword = string.Empty;
        await LoadAsync();
        StatusMessage = "Credential stored.";
    }

    [RelayCommand]
    private async Task RemoveCredentialAsync(string username)
    {
        await _vault.RemoveAsync(username);
        await LoadAsync();
        StatusMessage = $"Credential removed: {username}";
    }
}
