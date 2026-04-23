// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-23 | SESSION: M49-AutoDetect
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — HeadlessDetector (auto-detect headless environment)
 * *************************************************************************** */

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Detects whether the current runtime environment lacks a graphical display.
/// Used to automatically switch Front Door into headless/terminal mode.
/// </summary>
public static class HeadlessDetector
{
    /// <summary>
    /// Returns true when no display server is reachable or the session is
    /// known to be non-interactive (SSH, tmux, WSL without X11).
    /// </summary>
    public static bool IsHeadlessEnvironment()
    {
        // SSH session — always headless
        if (Environment.GetEnvironmentVariable("SSH_CONNECTION") != null)
            return true;

        // tmux / GNU screen session — typically headless
        if (Environment.GetEnvironmentVariable("TMUX") != null
            || Environment.GetEnvironmentVariable("SCREEN") != null)
            return true;

        // No DISPLAY set on Linux / macOS → no X11 or Wayland
        if (!OperatingSystem.IsWindows()
            && Environment.GetEnvironmentVariable("DISPLAY") == null)
            return true;

        // WSL without a forwarded display
        if (Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null
            && Environment.GetEnvironmentVariable("DISPLAY") == null)
            return true;

        return false;
    }
}
