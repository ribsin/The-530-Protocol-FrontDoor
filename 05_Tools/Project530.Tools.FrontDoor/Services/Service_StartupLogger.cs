// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: Cline-Agent003-DesignBay | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M63-FrontDoor-Fix
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Startup Logger
 * *************************************************************************** */

using System.Runtime.InteropServices;

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Logs startup diagnostics to file and console.
/// Path: ~/.config/Five30/FrontDoor/startup.log (Linux) or %APPDATA%\Five30\FrontDoor\startup.log (Windows)
/// </summary>
public sealed class Service_StartupLogger
{
    private readonly string _logFile;
    private readonly object _lock = new();

    public Service_StartupLogger()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDir = Path.Combine(appData, "Five30", "FrontDoor");
        
        // Fallback for Linux
        if (!Directory.Exists(configDir))
        {
            var home = Environment.GetEnvironmentVariable("HOME") ?? "/tmp";
            configDir = Path.Combine(home, ".config", "Five30", "FrontDoor");
        }
        
        Directory.CreateDirectory(configDir);
        _logFile = Path.Combine(configDir, "startup.log");
        
        // Clear old log on startup
        Clear();
    }

    public void Info(string message) => Log("INFO", message);
    
    public void Warn(string message) => Log("WARN", message);
    
    public void Error(string message) => Log("ERROR", message);
    
    public void Error(string message, Exception ex) => Log("ERROR", $"{message}: {ex.Message}\n{ex.StackTrace}");

    private void Log(string level, string message)
    {
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var line = $"[{timestamp}] [{level}] {message}";
        
        lock (_lock)
        {
            try
            {
#pragma warning disable RS0030
                File.AppendAllText(_logFile, line + Environment.NewLine);
#pragma warning restore RS0030
            }
            catch
            {
                // Ignore file write errors
            }
        }
        
        // Also write to console for TTY environments
        Console.WriteLine(line);
    }

    public void Clear()
    {
        lock (_lock)
        {
            try
            {
#pragma warning disable RS0030
                if (File.Exists(_logFile))
                {
                    File.Delete(_logFile);
                }
#pragma warning restore RS0030
            }
            catch
            {
                // Ignore
            }
        }
    }

    public string GetLogPath() => _logFile;
}