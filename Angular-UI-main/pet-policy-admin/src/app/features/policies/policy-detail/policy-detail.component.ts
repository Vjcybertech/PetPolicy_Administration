import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PolicyService } from '../../../services/policy.service';
import { ToastService } from '../../../services/toast.service';
import { PolicyDto } from '../../../models/policy.model';
import jsPDF from 'jspdf';

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


  downloadPolicySummary(): void {
  if (!this.policy) {
    return;
  }

  const doc = new jsPDF();

  let y = 20;

  doc.setFontSize(18);
  doc.text('Pet Insurance Policy Summary', 20, y);

  y += 15;

  doc.setFontSize(12);

  doc.text(`Policy Number: ${this.policy.policyNumber}`, 20, y);
  y += 10;

  doc.text(`Status: ${this.policy.status}`, 20, y);
  y += 10;

  doc.text(`Customer Name: ${this.policy.customerName}`, 20, y);
  y += 10;

  doc.text(`Customer Email: ${this.policy.customerEmail}`, 20, y);
  y += 10;

  doc.text(`Customer Phone: ${this.policy.customerPhone}`, 20, y);
  y += 10;

  doc.text(`Customer ID: ${this.policy.customerId}`, 20, y);
  y += 15;

  doc.setFontSize(14);
  doc.text('Pet Information', 20, y);

  y += 10;

  doc.setFontSize(12);
  doc.text(`Pet Name: ${this.policy.petName}`, 20, y);
  y += 10;

  doc.text(`Pet Type: ${this.policy.petType}`, 20, y);
  y += 10;

  doc.text(`Breed: ${this.policy.breed}`, 20, y);
  y += 10;

  doc.text(`Age: ${this.policy.age} Years`, 20, y);
  y += 15;

  doc.setFontSize(14);
  doc.text('Policy Details', 20, y);

  y += 10;

  doc.setFontSize(12);
  doc.text(`Premium: ${this.formatCurrency(this.policy.premium)}`, 20, y);
  y += 10;

  doc.text(`Payment Frequency: ${this.policy.paymentFrequency}`, 20, y);
  y += 10;

  doc.text(`Start Date: ${this.formatDate(this.policy.startDate)}`, 20, y);
  y += 10;

  doc.text(`End Date: ${this.formatDate(this.policy.endDate)}`, 20, y);
  y += 15;

  if (this.policy.coverages?.length) {
    doc.setFontSize(14);
    doc.text('Coverage Details', 20, y);

    y += 10;

    doc.setFontSize(12);

    this.policy.coverages.forEach((cov, index) => {
      doc.text(
        `${index + 1}. ${cov.coverageType} | Limit: ${this.formatCurrency(cov.annualLimit)}`,
        20,
        y
      );

      y += 8;

      doc.text(
        `Deductible: ${this.formatCurrency(cov.deductible)} | Reimbursement: ${cov.reimbursementPct}%`,
        25,
        y
      );

      y += 10;
    });
  }

  doc.save(`${this.policy.policyNumber}_Summary.pdf`);

  this.toastService.success('Policy PDF downloaded successfully.');
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
    return s === 'expiringsoon' ;
  }

  canCancel(): boolean {
    if (!this.policy) return false;
    const s = (this.policy.status || '').toLowerCase().replace(/\s+/g, '');
    return s !== 'cancelled' && s !== 'expired';
  }
}
