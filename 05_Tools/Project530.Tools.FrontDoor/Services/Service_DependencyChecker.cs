// [530-PROVENANCE] AI-GENERATED | AGENT_ID: Cline-Agent003-DesignBay | LAST-MODIFIED: 2026-04-23 | SECURITY_STATUS: STEEL-CHECK-PASSED

namespace Project530.Tools.FrontDoor.Services;

/// <summary>
/// Checks for required Linux dependencies and offers auto-installation.
/// </summary>
public class Service_DependencyChecker
{
    private readonly string[] _requiredPackages = 
    {
        "libgtk-3-0",
        "libgdk-pixbuf-2.0-0",
        "libatk1.0-0",
        "libatk-bridge2.0-0",
        "libx11-xcb1",
        "libxcb-util1",
        "libxcb-cursor0",
        "libskia-sharp",
        "libskia-sharp-data"
    };

    /// <summary>
    /// Checks if all required dependencies are installed.
    /// </summary>
    /// <returns>List of missing package names (empty if all present)</returns>
    public List<string> CheckMissingDependencies()
    {
        var missing = new List<string>();
        
        foreach (var package in _requiredPackages)
        {
            if (!IsPackageInstalled(package))
            {
                missing.Add(package);
            }
        }
        
        return missing;
    }

    /// <summary>
    /// Checks if a package is installed via dpkg.
    /// </summary>
    private bool IsPackageInstalled(string packageName)
    {
        try
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "dpkg",
                    Arguments = $"-s {packageName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            return process.ExitCode == 0 && output.Contains("Status: install ok installed");
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Attempts to install missing packages via apt-get.
    /// Returns true if installation succeeded.
    /// </summary>
    public bool TryInstallDependencies()
    {
        var missing = CheckMissingDependencies();
        if (missing.Count == 0)
        {
            Console.WriteLine("[Five30] All dependencies are installed.");
            return true;
        }

        Console.WriteLine("[Five30] Installing missing dependencies...");
        Console.WriteLine($"[Five30] Packages needed: {string.Join(", ", missing)}");
        
        try
        {
            // Update package list first
            var updateProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "sudo",
                    Arguments = "apt-get update",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            updateProcess.Start();
            updateProcess.WaitForExit();
            
            if (updateProcess.ExitCode != 0)
            {
                var error = updateProcess.StandardError.ReadToEnd();
                Console.WriteLine($"[Five30] Warning: apt-get update failed: {error}");
            }

            // Install packages
            var packages = string.Join(" ", missing);
            var installProcess = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "sudo",
                    Arguments = $"apt-get install -y {packages}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            
            installProcess.Start();
            var installError = installProcess.StandardError.ReadToEnd();
            installProcess.WaitForExit();
            
            if (installProcess.ExitCode == 0)
            {
                Console.WriteLine("[Five30] Dependencies installed successfully.");
                return true;
            }
            else
            {
                Console.WriteLine($"[Five30] Installation failed: {installError}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Five30] Error during installation: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Shows interactive prompt to install dependencies.
    /// </summary>
    public void ShowDependencyPrompt()
    {
        var missing = CheckMissingDependencies();
        if (missing.Count == 0)
        {
            return; // All good
        }

        Console.WriteLine();
        Console.WriteLine("+==========================================================+");
        Console.WriteLine("|  WARNING: Five30 Front Door - Missing Dependencies       |");
        Console.WriteLine("+==========================================================+");
        Console.WriteLine("|                                                          |");
        Console.WriteLine("|  The following packages are required for GUI mode:       |");
        
        foreach (var pkg in missing)
        {
            Console.WriteLine($"|    * {pkg,-48} |");
        }
        
        Console.WriteLine("|                                                          |");
        Console.WriteLine("|  You can install them automatically with sudo privileges. |");
        Console.WriteLine("+==========================================================+");
        Console.WriteLine("|                                                          |");
        Console.WriteLine("|  [1] Install Dependencies (recommended)                  |");
        Console.WriteLine("|  [2] Continue in Headless/Terminal Mode                  |");
        Console.WriteLine("|                                                          |");
        Console.WriteLine("+==========================================================+");
        Console.WriteLine();
        Console.Write("  Enter choice [1]: ");
        
        var choice = Console.ReadLine()?.Trim() ?? "1";
        
        if (choice == "1" || string.IsNullOrEmpty(choice))
        {
            Console.WriteLine();
            if (TryInstallDependencies())
            {
                Console.WriteLine("[Five30] Ready to run in GUI mode.");
            }
            else
            {
                Console.WriteLine("[Five30] Installation failed. Switching to headless mode.");
                choice = "2";
            }
        }
        
        if (choice == "2")
        {
            Console.WriteLine("[Five30] Switching to headless mode...");
            // Signal to Program.cs to run headless
            Environment.SetEnvironmentVariable("FIVE30_HEADLESS_MODE", "1");
        }
    }
}
