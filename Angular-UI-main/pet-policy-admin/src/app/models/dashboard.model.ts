export interface StatusSummaryDto {
  status: string;
  count: number;
  totalPremium: number;
}

export interface DashboardDto {
  totalPolicies: number;
  activeCount: number;
  expiringSoonCount: number;
  expiredCount: number;
  cancelledCount: number;
  pendingCount: number;
  totalWrittenPremium: number;
  statusBreakdown: StatusSummaryDto[];
}
