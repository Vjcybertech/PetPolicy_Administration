import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { QuoteService } from '../../services/quote.service';
import { ToastService } from '../../services/toast.service';
import { QuoteDto } from '../../models/quote.model';

@Component({
  selector: 'app-quotes',
  templateUrl: './quotes.component.html',
  styleUrls: ['./quotes.component.scss'],
  standalone: false
})
export class QuotesComponent implements OnInit {
  quotes: QuoteDto[] = [];
  filteredQuotes: QuoteDto[] = [];
  error = '';

  selectedStatus = '';
  searchTerm = '';

  // New Quote Modal
  showNewQuoteModal = false;
  creating = false;
  newQuote: Partial<QuoteDto> = {
    quoteId: `QT-${Math.floor(1000 + Math.random() * 9000)}`,
    customerId: 'CUST-100',
    customerName: '',
    customerEmail: '',
    customerPhone: '',
    petId: 'PET-100',
    petName: '',
    petType: 'Dog',
    breed: 'Labrador Retriever',
    age: 2,
    annualPremium: 500,
    paymentFrequency: 'Annual',
    status: 'Approved',
    coverageType: 'Comprehensive',
    annualLimit: 10000,
    deductible: 250,
    reimbursementPct: 80
  };

  constructor(
    private quoteService: QuoteService,
    private toastService: ToastService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadQuotes();
  }

  loadQuotes(): void {
    this.quoteService.getQuotes(this.selectedStatus || undefined).subscribe({
      next: (data) => {
        this.quotes = data;
        this.applyFilter();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load quotes.';
        this.cdr.detectChanges();
      }
    });
  }

  applyFilter(): void {
    let result = [...this.quotes];
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(q =>
        q.quoteId.toLowerCase().includes(term) ||
        q.customerName.toLowerCase().includes(term) ||
        q.petName.toLowerCase().includes(term)
      );
    }
    this.filteredQuotes = result;
  }

  createPolicyFromQuote(quoteId: string): void {
    this.router.navigate(['/create-policy'], { queryParams: { quoteId } });
  }

  openNewQuoteModal(): void {
    this.newQuote.quoteId = `QT-${Math.floor(1000 + Math.random() * 9000)}`;
    this.showNewQuoteModal = true;
  }

  closeNewQuoteModal(): void {
    this.showNewQuoteModal = false;
  }

  saveNewQuote(): void {
    if (!this.newQuote.customerName || !this.newQuote.petName) {
      this.toastService.warning('Please enter customer and pet names.');
      return;
    }
    this.creating = true;
    this.quoteService.createQuote(this.newQuote as QuoteDto).subscribe({
      next: (created) => {
        this.creating = false;
        this.showNewQuoteModal = false;
        this.toastService.success(`Quote ${created.quoteId} created successfully!`);
        this.loadQuotes();
      },
      error: (err) => {
        this.creating = false;
        this.toastService.error('Failed to create quote.');
        this.cdr.detectChanges();
      }
    });
  }

  formatCurrency(val: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(val);
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Approved': 'badge-active',
      'Converted': 'badge-pending',
      'Expired': 'badge-expired',
      'Pending': 'badge-expiring'
    };
    return map[status] || 'badge-pending';
  }
}
