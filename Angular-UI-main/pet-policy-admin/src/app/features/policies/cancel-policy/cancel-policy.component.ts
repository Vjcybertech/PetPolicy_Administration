import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PolicyService } from '../../../services/policy.service';
import { ToastService } from '../../../services/toast.service';
import { PolicyDto } from '../../../models/policy.model';

@Component({
  selector: 'app-cancel-policy',
  templateUrl: './cancel-policy.component.html',
  styleUrls: ['./cancel-policy.component.scss'],
  standalone: false
})
export class CancelPolicyComponent implements OnInit {
  policy: PolicyDto | null = null;
  cancelForm!: FormGroup;
  loading = true;
  submitting = false;
  error = '';
  successMessage = '';
  cancelledPolicy: PolicyDto | null = null;
  policyNumber = '';
  minCancellationDate = new Date().toISOString().split('T')[0];
  maxCancellationDate = '';

  cancellationReasons = [
    'Customer Request',
    'Non-Payment of Premium',
    'Pet Deceased',
    'Policy Transfer',
    'Fraud or Misrepresentation',
    'Underwriting Reason',
    'Other'
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private policyService: PolicyService,
    private toastService: ToastService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.policyNumber = this.route.snapshot.paramMap.get('policyNumber') || '';
    this.cancelForm = this.fb.group({
      cancellationDate: [new Date().toISOString().slice(0, 10), Validators.required],
      reason: ['', Validators.required],
      notes: ['']
    });

    const statePolicy=history.state?.policy as PolicyDto | undefined;
    if(statePolicy && statePolicy.policyNumber === this.policyNumber){
      this.policy=statePolicy;
    }


    this.loadPolicy();
  }

  loadPolicy(): void {
    this.policyService.getPolicyByNumber(this.policyNumber).subscribe({
      next: (data) => { this.policy = data; this.maxCancellationDate = data.endDate.split('T')[0];  this.loading = false; },
      error: () => { this.error = 'Policy not found.'; this.loading = false; }
    });
  }

  onSubmit(): void {
    if (this.cancelForm.invalid) return;
    this.submitting = true;
    this.error = '';
    const dto = {
      ...this.cancelForm.value,
      cancellationDate: new Date(this.cancelForm.value.cancellationDate).toISOString()
    };
    this.policyService.cancelPolicy(this.policyNumber, dto).subscribe({
      next: (cancelled) => {
        this.cancelledPolicy = cancelled;
        this.successMessage = `Policy ${this.policyNumber} has been successfully cancelled.`;
        this.submitting = false;
        this.toastService.warning(`Policy ${this.policyNumber} cancelled.`);
      },
      error: (err) => {
        this.error = err?.error?.message || 'Cancellation failed. Please try again.';
        this.submitting = false;
        this.toastService.error('Cancellation failed.');
      }
    });
  }

  viewPolicy(): void {
    this.router.navigate(['/policies', this.policyNumber]);
  }

  goBack(): void {
    this.router.navigate(['/policies', this.policyNumber]);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value || 0);
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
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
}
