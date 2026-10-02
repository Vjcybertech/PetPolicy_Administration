using System.Collections.Generic;

namespace PolicyAdmin.API.DTOs
{
    public class DashboardDto
    {
        public int TotalPolicies { get; set; }
        public int ActiveCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public int ExpiredCount { get; set; }
        public int CancelledCount { get; set; }
        public int PendingCount { get; set; }
        public decimal TotalWrittenPremium { get; set; }
        public List<StatusSummaryDto> StatusBreakdown { get; set; } = new();
    }

    public class StatusSummaryDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalPremium { get; set; }
    }
}
