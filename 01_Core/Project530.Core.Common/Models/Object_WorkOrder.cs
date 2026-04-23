// [530-PROVENANCE]
/* ***************************************************************************
 * PROJECT: 5:30 Protocol | COMPLIANCE: 2026.AI-ACT.ARTICLE-50
 * PROVENANCE: AI-GENERATED
 * AGENT_ID: GitHub-Copilot-Claude-Opus-4.6 | AUTHOR_ID: 530-Protocol-Team
 * LAST_MODIFIED: 2026-04-07
 * SECURITY_STATUS: STEEL-CHECK-PASSED
 * *************************************************************************** */

using System;

namespace Project530.Core.Common.Models;

/// <summary>
/// Queue-specific envelope that wraps a WorkPlanItem with dispatch metadata.
/// This is the unit of transfer across the distributed work order queue,
/// carrying lifecycle tracking fields (ID, timestamps, retry count, status)
/// that the domain-level WorkPlanItem does not concern itself with.
/// </summary>
public sealed class Object_WorkOrder
{
    /// <summary>
    /// Unique identifier for this queued work order.
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// The originating work plan that produced this order.
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// The domain work item (work order + agent assignments) to be executed.
    /// </summary>
    public WorkPlanItem Item { get; set; } = new();

    /// <summary>
    /// Queue lifecycle status: Pending, Processing, Completed, Failed.
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Number of times this order has been attempted.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Maximum number of retry attempts before the order is marked as Failed.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// UTC timestamp when this order was first enqueued.
    /// </summary>
    public DateTime EnqueuedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp when processing started, or null if not yet picked up.
    /// </summary>
    public DateTime? StartedUtc { get; set; }

    /// <summary>
    /// UTC timestamp when processing completed (success or final failure).
    /// </summary>
    public DateTime? CompletedUtc { get; set; }

    /// <summary>
    /// Human-readable error description if the order failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new Object_WorkOrder from a WorkPlanItem with a generated ID.
    /// </summary>
    public static Object_WorkOrder FromItem(WorkPlanItem item, string planId)
    {
        return new Object_WorkOrder
        {
            OrderId = Guid.NewGuid().ToString("N"),
            PlanId = planId,
            Item = item,
            Status = "Pending",
            RetryCount = 0,
            EnqueuedUtc = DateTime.UtcNow
        };
    }
}
