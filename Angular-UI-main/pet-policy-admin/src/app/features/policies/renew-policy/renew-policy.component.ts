import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PolicyService } from '../../../services/policy.service';
import { ToastService } from '../../../services/toast.service';
import { PolicyDto } from '../../../models/policy.model';

@Component({
  selector: 'app-renew-policy',
  templateUrl: './renew-policy.component.html',
  styleUrls: ['./renew-policy.component.scss'],
  standalone: false
})
export class RenewPolicyComponent implements OnInit {
  policy: PolicyDto | null = null;
  renewForm!: FormGroup;
  loading = false;
  submitting = false;
  error = '';
  successMessage = '';
  renewedPolicy: PolicyDto | null = null;
  policyNumber = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private policyService: PolicyService,
    private toastService: ToastService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.policyNumber = this.route.snapshot.paramMap.get('policyNumber') || '';
    this.renewForm = this.fb.group({
      proposedPremium: ['', [Validators.required, Validators.min(0.01)]],
      extensionYears: [1, [Validators.required, Validators.min(1), Validators.max(5)]],
      notes: ['Standard annual policy renewal']
    });

    const statePolicy = history.state?.policy as PolicyDto | undefined;
    if(statePolicy && statePolicy.policyNumber ===this.policyNumber){
      this.policy=statePolicy;
      this.renewForm.patchValue({ proposedPremium: statePolicy.premium });
    }


    this.loadPolicy();
  }

  loadPolicy(): void {
    this.policyService.getPolicyByNumber(this.policyNumber).subscribe({
      next: (data) => {
        this.policy = data;
        if(!this.renewForm.get('proposedPremium')?.dirty) {
        this.renewForm.patchValue({ proposedPremium: data.premium });
        }
        this.loading=false;
      },
      error: () => {
        this.error = 'Policy not found.';
        this.loading = false;
      }
    });
  }

  get proposedPremium(): number {
    return this.renewForm.get('proposedPremium')?.value || 0;
  }

  get premiumChange(): number {
    if (!this.policy) return 0;
    return this.proposedPremium - this.policy.premium;
  }

  get premiumChangePct(): number {
    if (!this.policy || this.policy.premium === 0) return 0;
    return (this.premiumChange / this.policy.premium) * 100;
  }

  onSubmit(): void {
    if (this.renewForm.invalid) return;
    this.submitting = true;
    this.error = '';
    const dto = this.renewForm.value;
    this.policyService.renewPolicy(this.policyNumber, dto).subscribe({
      next: (renewed) => {
        this.renewedPolicy = renewed;
        this.successMessage = `Policy ${this.policyNumber} has been successfully renewed!`;
        this.submitting = false;
        this.toastService.success(`Policy ${this.policyNumber} renewed!`);
      },
      error: (err) => {
        this.error = err?.error?.message || 'Renewal failed. Please try again.';
        this.submitting = false;
        this.toastService.error('Renewal failed.');
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
