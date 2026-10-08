import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { PolicyService } from '../../services/policy.service';
import { DashboardDto } from '../../models/dashboard.model';
import { PolicyDto } from '../../models/policy.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: false
})
export class DashboardComponent implements OnInit {
  dashboard: DashboardDto = {
    totalPolicies: 0,
    activeCount: 0,
    expiringSoonCount: 0,
    expiredCount: 0,
    cancelledCount: 0,
    pendingCount: 0,
    totalWrittenPremium: 0,
    statusBreakdown: []
  };
  recentPolicies: PolicyDto[] = [];
  expiringPolicies: PolicyDto[] = [];
  error = '';

  constructor(
    private policyService: PolicyService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
    this.loadRecentPolicies();
    this.loadExpiringPolicies();
  }

  loadDashboard(): void {
    this.policyService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load dashboard metrics. Please ensure backend is running.';
        this.cdr.detectChanges();
      }
    });
  }

  loadRecentPolicies(): void {
    this.policyService.getPolicies(undefined, 1, 6).subscribe({
      next: (data) => {
        this.recentPolicies = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  loadExpiringPolicies(): void {
    this.policyService.getExpiringPolicies(30).subscribe({
      next: (data) => {
        this.expiringPolicies = data.slice(0, 5);
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  navigateToPolicies(status?: string): void {
    if (status) {
      this.router.navigate(['/policies'], { queryParams: { status } });
    } else {
      this.router.navigate(['/policies']);
    }
  }

  viewPolicy(policyNumber: string): void {
    this.router.navigate(['/policies', policyNumber]);
  }

  getStatusClass(status: string): string {
    const s = (status || '').toLowerCase().replace(/\s+/g, '');
    const map: Record<string, string> = {
      'active': 'badge-active',
      'expired': 'badge-expired',
      'cancelled': 'badge-cancelled',
      'pending': 'badge-pending',
      'expiringsoon': 'badge-expiring'
    };
    return map[s] || 'badge-pending';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value || 0);
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }
}
