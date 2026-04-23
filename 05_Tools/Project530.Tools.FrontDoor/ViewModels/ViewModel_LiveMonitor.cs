// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — ViewModel_LiveMonitor
 * *************************************************************************** */

using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Project530.Core.Common.Models;

namespace Project530.Tools.FrontDoor.ViewModels;

public sealed partial class ViewModel_LiveMonitor : ViewModel_Base, IAsyncDisposable
{
    private readonly string _backendUrl;
    private HubConnection? _hub;
    private readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    [ObservableProperty] private int _activeTab = 0;
    [ObservableProperty] private string _healthStatus = "connecting";
    [ObservableProperty] private int _tokenBalance = 0;
    [ObservableProperty] private double _msgPerSec = 0;

    public ObservableCollection<string> LogLines { get; } = new();
    public ObservableCollection<Object_WorkPlanDto> Plans { get; } = new();
    public ObservableCollection<Object_AgentStatusDto> Roster { get; } = new();
    public ObservableCollection<string> BrainPool { get; } = new();

    public ViewModel_LiveMonitor(string backendUrl)
    {
        _backendUrl = backendUrl;
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
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                LogLines.Add(entry);
                while (LogLines.Count > 200) LogLines.RemoveAt(0);
            });
        });

        _hub.On<Object_WorkPlanDto>("ReceiveWorkPlan", plan =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => Plans.Insert(0, plan));
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
        using var http = new HttpClient { BaseAddress = new Uri(_backendUrl), Timeout = TimeSpan.FromSeconds(5) };
        await FetchJsonAsync<List<Object_WorkPlanDto>>(http, "/api/operations/plans", ct,
            items => { Plans.Clear(); foreach (var p in items) Plans.Add(p); });
        await FetchJsonAsync<List<Object_AgentStatusDto>>(http, "/api/foreman/roster", ct,
            items => { Roster.Clear(); foreach (var a in items) Roster.Add(a); });
        await FetchJsonAsync<List<string>>(http, "/api/foreman/models", ct,
            items => { BrainPool.Clear(); foreach (var m in items) BrainPool.Add(m); });
        await FetchHealthAsync(http, ct);
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
        Action<T> apply) where T : class
    {
        try
        {
            var raw = await http.GetStringAsync(path, ct);
            var value = JsonSerializer.Deserialize<T>(raw, _jsonOpts);
            if (value != null) apply(value);
        }
        catch { /* non-fatal — UI shows stale data */ }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
            await _hub.DisposeAsync();
    }
}
