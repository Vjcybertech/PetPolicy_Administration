import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PolicyService } from '../../../services/policy.service';
import { ToastService } from '../../../services/toast.service';
import { PolicyDto } from '../../../models/policy.model';

@Component({
  selector: 'app-policy-detail',
  templateUrl: './policy-detail.component.html',
  styleUrls: ['./policy-detail.component.scss'],
  standalone: false
})
export class PolicyDetailComponent implements OnInit {
  policy: PolicyDto | null = null;
  loading = true;
  error = '';
  policyNumber = '';
  activeTab: 'summary' | 'pet' | 'coverage' | 'transactions' | 'renewals' = 'summary';

  // Modal for Change Coverage / Edit Details
  showEditModal = false;
  editCustomerEmail = '';
  editCustomerPhone = '';
  editPaymentFrequency = 'Annual';
  updating = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private policyService: PolicyService,
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
  this.route.paramMap.subscribe(params => {
    this.policyNumber = params.get('policyNumber') || '';
    this.loadPolicy();
  });
}

  loadPolicy(): void {
    this.loading = true;
    this.policyService.getPolicyByNumber(this.policyNumber).subscribe({
      next: (data) => {
        this.policy = data;
        this.editCustomerEmail = data.customerEmail;
        this.editCustomerPhone = data.customerPhone;
        this.editPaymentFrequency = data.paymentFrequency;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = `Policy '${this.policyNumber}' not found.`;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openEditModal(): void {
    if (this.policy) {
      this.editCustomerEmail = this.policy.customerEmail;
      this.editCustomerPhone = this.policy.customerPhone;
      this.editPaymentFrequency = this.policy.paymentFrequency;
      this.showEditModal = true;
    }
  }

  closeEditModal(): void {
    this.showEditModal = false;
  }

  savePolicyChanges(): void {
    if (!this.policy) return;
    this.updating = true;
    this.policyService.updatePolicy(this.policyNumber, {
      customerEmail: this.editCustomerEmail,
      customerPhone: this.editCustomerPhone,
      paymentFrequency: this.editPaymentFrequency
    }).subscribe({
      next: (updated) => {
        this.policy = updated;
        this.updating = false;
        this.showEditModal = false;
        this.toastService.success(`Policy ${this.policyNumber} updated successfully.`);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.updating = false;
        this.toastService.error('Failed to update policy.');
        this.cdr.detectChanges();
      }
    });
  }

  exportSummary(): void {
    this.policyService.exportPolicies().subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Policy_${this.policyNumber}_Summary.xlsx`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastService.success('Policy summary exported to Excel.');
      },
      error: (err) => {
        this.toastService.error('Export failed.');
      }
    });
  }

  renewPolicy(): void {
    this.router.navigate(['/policies', this.policyNumber, 'renew'], {state: {policy:this.policy}});
  }

  cancelPolicy(): void {
    this.router.navigate(['/policies', this.policyNumber, 'cancel'], {state: {policy: this.policy}});
  }

  goBack(): void {
    this.router.navigate(['/policies']);
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

  canRenew(): boolean {
    if (!this.policy) return false;
    const s = (this.policy.status || '').toLowerCase().replace(/\s+/g, '');
    return s === 'expiringsoon' || s ==='active';
  }

  canCancel(): boolean {
    if (!this.policy) return false;
    const s = (this.policy.status || '').toLowerCase().replace(/\s+/g, '');
    return s !== 'cancelled' && s !== 'expired';
  }
}
