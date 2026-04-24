// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M50-FrontDoor-Fixes
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — ViewModel_LiveMonitor
 * *************************************************************************** */

using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Project530.Core.Common.Models;
using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.ViewModels;

public sealed partial class ViewModel_LiveMonitor : ViewModel_Base, IAsyncDisposable
{
    private readonly string _backendUrl;
    private HubConnection? _hub;
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [ObservableProperty] private int _activeTab = 0;
    [ObservableProperty] private string _healthStatus = "connecting";
    [ObservableProperty] private int _tokenBalance = 0;
    [ObservableProperty] private double _msgPerSec = 0;

    // Dashboard stats
    [ObservableProperty] private int _userCount = 0;
    [ObservableProperty] private int _profileCount = 0;
    [ObservableProperty] private int _modCount = 0;
    [ObservableProperty] private int _modPackCount = 0;
    [ObservableProperty] private int _gameRequestCount = 0;
    [ObservableProperty] private int _testerResultCount = 0;
    [ObservableProperty] private int _failedModPackCount = 0;
    [ObservableProperty] private int _failedModCount = 0;

    // AI Agent APIs — add / remove fields
    [ObservableProperty] private string _newProviderName = string.Empty;
    [ObservableProperty] private string _newEndpoint = string.Empty;
    [ObservableProperty] private string _newApiKey = string.Empty;
    [ObservableProperty] private string _aiAgentStatusMessage = string.Empty;

    // Tab navigation
    [RelayCommand]
    private void SelectTab(string tabIndex)
    {
        if (int.TryParse(tabIndex, out var index))
            ActiveTab = index;
    }

    public ObservableCollection<string> LogLines { get; } = new();
    public ObservableCollection<Object_WorkPlanDto> Plans { get; } = new();
    public ObservableCollection<Object_AgentStatusDto> Roster { get; } = new();
    public ObservableCollection<BrainPoolModel> BrainPool { get; } = new();
    public ObservableCollection<Object_AiAgentConfigEntry> AiAgents { get; } = new();

    public ViewModel_LiveMonitor(string backendUrl)
    {
        _backendUrl = backendUrl;
        _http = new HttpClient { BaseAddress = new Uri(_backendUrl), Timeout = TimeSpan.FromSeconds(10) };
    }

    [RelayCommand]
    private async Task ConnectAsync(CancellationToken ct)
    {
        await FetchAllAsync(ct);

        _hub = new HubConnectionBuilder()
            .WithUrl($"{_backendUrl}/factoryhub")
            .WithAutomaticReconnect()
            .Build();

        _hub.On<string>("ReceiveLogEntry", entry =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                LogLines.Add(entry);
                while (LogLines.Count > 200) LogLines.RemoveAt(0);
            });
        });

        _hub.On<Object_WorkPlanDto>("ReceiveWorkPlan", plan =>
        {
            Dispatcher.UIThread.Post(() => Plans.Insert(0, plan));
        });

        try
        {
            await _hub.StartAsync(ct);
            HealthStatus = "connected";
        }
        catch
        {
            HealthStatus = "SignalR offline";
        }
    }

    private async Task FetchAllAsync(CancellationToken ct)
    {
        await FetchJsonAsync<List<Object_WorkPlanDto>>(_http, "/api/operations/plans", ct,
            items => { Plans.Clear(); foreach (var p in items) Plans.Add(p); });
        await FetchJsonAsync<List<Object_AgentStatusDto>>(_http, "/api/foreman/roster", ct,
            items => { Roster.Clear(); foreach (var a in items) Roster.Add(a); });
        await FetchJsonAsync<List<BrainPoolModel>>(_http, "/api/foreman/models", ct,
            items => { BrainPool.Clear(); foreach (var m in items) BrainPool.Add(m); });
        await FetchJsonAsync<List<Object_AiAgentConfigEntry>>(_http, "/api/foreman/ai-agents", ct,
            items => { AiAgents.Clear(); foreach (var a in items) AiAgents.Add(a); },
            logFailure: false);
        await FetchJsonAsync<DashboardStatsDto>(_http, "/api/dashboard/stats", ct, stats =>
        {
            UserCount = stats.UserCount;
            ProfileCount = stats.ProfileCount;
            ModCount = stats.ModCount;
            ModPackCount = stats.ModPackCount;
            GameRequestCount = stats.GameRequestCount;
            TesterResultCount = stats.TesterResultCount;
            FailedModPackCount = stats.FailedModPackCount;
            FailedModCount = stats.FailedModCount;
        });
        await FetchHealthAsync(_http, ct);
    }

    private async Task FetchHealthAsync(HttpClient http, CancellationToken ct)
    {
        try
        {
            var raw = await http.GetStringAsync("/api/health", ct);
            HealthStatus = raw.Contains("healthy", StringComparison.OrdinalIgnoreCase) ? "healthy" : "degraded";
        }
        catch { HealthStatus = "offline"; }
    }

    private async Task FetchJsonAsync<T>(
        HttpClient http,
        string path,
        CancellationToken ct,
        Action<T> apply,
        bool logFailure = true) where T : class
    {
        try
        {
            var raw = await http.GetStringAsync(path, ct);
            var value = JsonSerializer.Deserialize<T>(raw, _jsonOpts);
            if (value != null) apply(value);
        }
        catch when (logFailure)
        {
            /* non-fatal — UI shows stale data */
        }
    }

    [RelayCommand]
    private async Task AddAiAgentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProviderName) ||
            string.IsNullOrWhiteSpace(NewEndpoint) ||
            string.IsNullOrWhiteSpace(NewApiKey))
        {
            AiAgentStatusMessage = "Provider, endpoint, and API key are all required.";
            return;
        }

        var entry = new
        {
            provider = NewProviderName,
            endpoint = NewEndpoint,
            apiKey = NewApiKey
        };

        try
        {
            var json = JsonSerializer.Serialize(entry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("/api/foreman/ai-agents", content);

            if (resp.IsSuccessStatusCode)
            {
                AiAgentStatusMessage = $"Added: {NewProviderName}";
                NewProviderName = string.Empty;
                NewEndpoint = string.Empty;
                NewApiKey = string.Empty;

                // Refresh the list
                await FetchJsonAsync<List<Object_AiAgentConfigEntry>>(_http, "/api/foreman/ai-agents",
                    CancellationToken.None, items =>
                    {
                        AiAgents.Clear();
                        foreach (var a in items) AiAgents.Add(a);
                    });
            }
            else
            {
                AiAgentStatusMessage = $"Failed to add agent (HTTP {(int)resp.StatusCode})";
            }
        }
        catch (Exception ex)
        {
            AiAgentStatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RemoveAiAgentAsync(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider)) return;

        try
        {
            var encoded = Uri.EscapeDataString(provider);
            var resp = await _http.DeleteAsync($"/api/foreman/ai-agents/{encoded}");

            if (resp.IsSuccessStatusCode)
            {
                var toRemove = AiAgents.FirstOrDefault(a => a.Provider == provider);
                if (toRemove != null) AiAgents.Remove(toRemove);
                AiAgentStatusMessage = $"Removed: {provider}";
            }
            else
            {
                AiAgentStatusMessage = $"Failed to remove {provider} (HTTP {(int)resp.StatusCode})";
            }
        }
        catch (Exception ex)
        {
            AiAgentStatusMessage = $"Error: {ex.Message}";
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
            await _hub.DisposeAsync();
    }
}

/// <summary>Enriched model entry for Brain Pool display with provider/status context.</summary>
public sealed class BrainPoolModel
{
    public string ModelId { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";

    public static BrainPoolModel FromString(string raw)
    {
        var provider = Object_AiAgentConfigEntry.ExtractProvider(raw);
        return new BrainPoolModel
        {
            ModelId = raw,
            Provider = provider,
            Status = "Active"
        };
    }
}