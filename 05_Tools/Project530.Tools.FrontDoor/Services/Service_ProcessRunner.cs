// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Service_ProcessRunner
 * *************************************************************************** */

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Project530.Tools.FrontDoor.Models;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Runs a process and streams its combined stdout/stderr output line by line.
/// Cross-platform: works on both Windows and Linux/macOS.
/// </summary>
public sealed class Service_ProcessRunner : I_ProcessRunner
{
    public async IAsyncEnumerable<string> RunAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        [EnumeratorCancellation] CancellationToken ct)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.Start();

        var stdoutTask = ReadLinesAsync(process.StandardOutput, ct);
        var stderrTask = ReadLinesAsync(process.StandardError, ct);

        await foreach (var line in MergeAsync(stdoutTask, stderrTask, ct))
        {
            yield return line;
        }

        await process.WaitForExitAsync(ct);
    }

    private static async IAsyncEnumerable<string> ReadLinesAsync(
        System.IO.TextReader reader,
        [EnumeratorCancellation] CancellationToken ct)
    {
        string? line;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            if (!string.IsNullOrWhiteSpace(line))
                yield return line;
        }
    }

    private static async IAsyncEnumerable<string> MergeAsync(
        IAsyncEnumerable<string> source1,
        IAsyncEnumerable<string> source2,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var channel = System.Threading.Channels.Channel.CreateUnbounded<string>();

        async Task Pump(IAsyncEnumerable<string> src)
        {
            await foreach (var item in src.WithCancellation(ct))
                await channel.Writer.WriteAsync(item, ct);
        }

        var t1 = Pump(source1);
        var t2 = Pump(source2);
        _ = Task.WhenAll(t1, t2).ContinueWith(_ => channel.Writer.Complete(), CancellationToken.None);

        await foreach (var item in channel.Reader.ReadAllAsync(ct))
            yield return item;
    }
}
