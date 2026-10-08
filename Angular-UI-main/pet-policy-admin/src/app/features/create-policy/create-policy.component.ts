import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { QuoteService } from '../../services/quote.service';
import { PolicyService } from '../../services/policy.service';
import { ToastService } from '../../services/toast.service';
import { QuoteDto } from '../../models/quote.model';
import { PolicyDto } from '../../models/policy.model';

@Component({
  selector: 'app-create-policy',
  templateUrl: './create-policy.component.html',
  styleUrls: ['./create-policy.component.scss'],
  standalone: false
})
export class CreatePolicyComponent implements OnInit {
  step: 'lookup' | 'form' | 'success' = 'lookup';

  quoteId = '';
  quoteLookupLoading = false;
  quoteLookupError = '';
  quote: QuoteDto | null = null;

  policyForm!: FormGroup;
  submitting = false;
  submitError = '';
  createdPolicy: PolicyDto | null = null;

  coverageTypes = ['Comprehensive', 'Accident Only', 'Wellness', 'Illness Only'];
  paymentFrequencies = ['Annual', 'Monthly', 'Quarterly', 'Semi-Annual'];

  constructor(
    private fb: FormBuilder,
    private quoteService: QuoteService,
    private policyService: PolicyService,
    private toastService: ToastService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.policyForm = this.fb.group({
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      paymentFrequency: ['Annual', Validators.required],
      annualPremium: ['', [Validators.required, Validators.min(0.01)]],
      coverageType: ['Comprehensive', Validators.required],
      annualLimit: ['', [Validators.required, Validators.min(0)]],
      deductible: ['', [Validators.required, Validators.min(0)]],
      reimbursementPct: ['', [Validators.required, Validators.min(0), Validators.max(100)]]
    });

    this.route.queryParams.subscribe(params => {
      if (params['quoteId']) {
        this.quoteId = params['quoteId'];
        this.lookupQuote();
      }
    });
  }

  lookupQuote(): void {
    if (!this.quoteId.trim()) {
      this.quoteLookupError = 'Please enter a quote ID.';
      return;
    }
    this.quoteLookupLoading = true;
    this.quoteLookupError = '';
    this.quote = null;

    this.quoteService.getQuoteById(this.quoteId.trim()).subscribe({
      next: (q) => {
        this.quote = q;
        this.quoteLookupLoading = false;
        if (q.status !== 'Approved') {
          this.quoteLookupError = `This quote has status "${q.status}" and is not eligible for policy creation. Only Approved quotes can be used.`;
          this.quote = null;
        } else {
          const today = new Date();
          const nextYear = new Date(today);
          nextYear.setFullYear(nextYear.getFullYear() + 1);
          this.policyForm.patchValue({
            annualPremium: q.annualPremium,
            paymentFrequency: q.paymentFrequency,
            coverageType: q.coverageType,
            annualLimit: q.annualLimit,
            deductible: q.deductible,
            reimbursementPct: q.reimbursementPct,
            startDate: today.toISOString().slice(0, 10),
            endDate: nextYear.toISOString().slice(0, 10)
          });
          this.step = 'form';
        }
      },
      error: (err) => {
        this.quoteLookupError = `Quote "${this.quoteId}" not found. Please check the quote number and try again.`;
        this.quoteLookupLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.policyForm.invalid || !this.quote) return;
    this.submitting = true;
    this.submitError = '';

    const formValue = this.policyForm.value;
    const dto = {
      quoteId: this.quote.quoteId,
      startDate: new Date(formValue.startDate).toISOString(),
      endDate: new Date(formValue.endDate).toISOString(),
      paymentFrequency: formValue.paymentFrequency,
      annualPremium: Number(formValue.annualPremium),
      coverageType: formValue.coverageType,
      annualLimit: Number(formValue.annualLimit),
      deductible: Number(formValue.deductible),
      reimbursementPct: Number(formValue.reimbursementPct)
    };

    this.policyService.createPolicy(dto).subscribe({
      next: (policy) => {
        this.createdPolicy = policy;
        this.step = 'success';
        this.submitting = false;
        this.toastService.success(`Policy ${policy.policyNumber} created successfully!`);
      },
      error: (err) => {
        this.submitError = err?.error?.message || 'Policy creation failed. Please try again.';
        this.submitting = false;
        this.toastService.error('Failed to create policy.');
      }
    });
  }

  resetForm(): void {
    this.step = 'lookup';
    this.quote = null;
    this.quoteId = '';
    this.quoteLookupError = '';
    this.policyForm.reset({
      paymentFrequency: 'Annual',
      coverageType: 'Comprehensive'
    });
  }

  viewCreatedPolicy(): void {
    if (this.createdPolicy) {
      this.router.navigate(['/policies', this.createdPolicy.policyNumber]);
    }
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
  }
}
