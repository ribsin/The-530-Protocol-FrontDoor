// [530-PROVENANCE]
/* **************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: HUMAN-REFINED
 * AGENT_ID: Gemini-Code-Assist-v1
 * SECURITY_STATUS: STEEL-CHECK-PENDING
 * ************************************************************************** */

namespace Project530.Core.Common.Models;

public sealed class Result<T>(T? value, Error? error, string? statusMessage)
{
    public T? Value { get; } = value;
    public Error? Error { get; } = error;
    public bool IsSuccess => Error is null;

    public string StatusMessage { get; init; } = statusMessage ?? (error is null ? "OK" : error.Description);

    public static Result<T> Ok(T value, string? status = "OK") => new(value, null, status);
    public static Result<T> Fail(string error, string? status = null) => new(default, new Error("ERR", error), status ?? error);
    public static Result<T> Fail(Error error, string? status = null) => new(default, error, status ?? error.Description);
}
