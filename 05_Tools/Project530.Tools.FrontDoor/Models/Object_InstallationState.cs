// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Sonnet-4.6 | AUTHOR_ID: Five30-Protocol-Team
 * LAST_MODIFIED: 2026-04-24 | SESSION: M48-Phase3-FrontDoor
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * DOCUMENT: Front Door — Installation State Model
 * *************************************************************************** */

namespace Project530.Tools.FrontDoor.Models;

/// <summary>The 7 ordered steps of the Setup Wizard.</summary>
public enum InstallStep
{
    Welcome = 0,
    DependencyCheck = 1,
    GitAuth = 2,
    VaultKeys = 3,
    Clone = 4,
    DockerUp = 5,
    HealthCheck = 6
}

/// <summary>Progress event emitted by Service_FactoryInstaller.</summary>
public sealed record InstallProgress(
    InstallStep Step,
    string Message,
    int PercentComplete,
    bool IsError = false
);

/// <summary>Final report returned after a successful installation.</summary>
public sealed record InstallationReport(
    string RepoPath,
    string BackendUrl,
    DateTimeOffset CompletedAt
);

/// <summary>Result container used across service operations.</summary>
public sealed class Result
{
    public bool IsSuccess { get; private init; }
    public string? ErrorMessage { get; private init; }

    public static Result Ok() => new() { IsSuccess = true };
    public static Result Fail(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public sealed class Result<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? ErrorMessage { get; private init; }

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
