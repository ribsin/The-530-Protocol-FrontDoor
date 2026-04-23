// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Service_BackendHealthPoller
 * *************************************************************************** */

using System.Runtime.CompilerServices;
using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Polls the backend's /api/health endpoint at a fixed interval.
/// Yields status strings until "healthy" is observed or the timeout elapses.
/// </summary>
public sealed class Service_BackendHealthPoller : I_BackendHealthPoller
{
    public async IAsyncEnumerable<string> PollAsync(
        Uri baseUrl,
        TimeSpan interval,
        TimeSpan timeout,
        [EnumeratorCancellation] CancellationToken ct)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var deadline = DateTimeOffset.UtcNow + timeout;
        var healthUrl = new Uri(baseUrl, "/api/health");

        while (DateTimeOffset.UtcNow < deadline && !ct.IsCancellationRequested)
        {
            string status;
            try
            {
                var response = await http.GetStringAsync(healthUrl, ct);
                status = response.Contains("healthy", StringComparison.OrdinalIgnoreCase)
                    ? "healthy"
                    : "starting";
            }
            catch
            {
                status = "offline";
            }

            yield return status;

            if (status == "healthy")
                yield break;

            await Task.Delay(interval, ct);
        }

        yield return "timeout";
    }
}
