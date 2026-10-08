import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { PolicyService } from '../../../services/policy.service';
import { ToastService } from '../../../services/toast.service';
import { PolicyDto } from '../../../models/policy.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-policies-list',
  templateUrl: './policies-list.component.html',
  styleUrls: ['./policies-list.component.scss'],
  standalone: false
})
export class PoliciesListComponent implements OnInit {
  policies: PolicyDto[] = [];
  filteredPolicies: PolicyDto[] = [];
  error = '';

  searchTerm = '';
  selectedStatus = '';
  pageNumber = 1;
  pageSize = 25;

  // Normalized status list
  statusOptions = [
    { label: 'All Statuses', value: '' },
    { label: 'Active', value: 'Active' },
    { label: 'Expiring Soon', value: 'Expiring Soon' },
    { label: 'Expired', value: 'Expired' },
    { label: 'Cancelled', value: 'Cancelled' },
    { label: 'Pending', value: 'Pending' }
  ];

  // Excel Import Modal
  showImportModal = false;
  selectedFile: File | null = null;
  importing = false;

  constructor(
    private policyService: PolicyService,
    private toastService: ToastService,
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      let st = params['status'] || '';
      // Map potential non-spaced url param 'ExpiringSoon' to 'Expiring Soon'
      if (st.toLowerCase() === 'expiringsoon') {
        st = 'Expiring Soon';
      }
      this.selectedStatus = st;
      this.loadPolicies();
    });
  }

  loadPolicies(): void {
    this.error = '';
    // Request policies with the selected status
    this.policyService.getPolicies(this.selectedStatus || undefined, this.pageNumber, this.pageSize).subscribe({
      next: (data) => {
        this.policies = data;
        this.applyFilters();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to load policies. Is the backend running?';
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void {
    let result = [...this.policies];

    if (this.selectedStatus) {
      const match = this.selectedStatus.toLowerCase().replace(/\s+/g, '');
      result = result.filter(p => (p.status || '').toLowerCase().replace(/\s+/g, '') === match);
    }

    if (this.searchTerm) {
      const t = this.searchTerm.toLowerCase();
      result = result.filter(p =>
        p.policyNumber.toLowerCase().includes(t) ||
        p.customerName.toLowerCase().includes(t) ||
        p.customerEmail.toLowerCase().includes(t) ||
        p.petName.toLowerCase().includes(t) ||
        p.breed.toLowerCase().includes(t)
      );
    }
    this.filteredPolicies = result;
  }

  onFilterChange(): void {
    this.pageNumber = 1;
    this.loadPolicies();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  quickSetStatus(status: string): void {
    this.selectedStatus = status;
    this.onFilterChange();
  }

  viewPolicy(policyNumber: string): void {
    this.router.navigate(['/policies', policyNumber]);
  }

  renewPolicy(policyNumber: PolicyDto | string, event: Event): void {
    event.stopPropagation();
    if(typeof policyNumber === 'string'){
    this.router.navigate(['/policies', policyNumber, 'renew']);
  } else {
    this.router.navigate(['/policies', policyNumber.policyNumber, 'renew'], {state: {policy:policyNumber}});
  }
}

  cancelPolicy(policyNumber: PolicyDto | string, event: Event): void {
    event.stopPropagation();
    if(typeof policyNumber === 'string'){
    this.router.navigate(['/policies', policyNumber, 'cancel']);
  }else {
    this.router.navigate(['/policies', policyNumber.policyNumber, 'cancel'], {state: {policy:policyNumber}});
  }
}

  exportToExcel(): void {
    this.policyService.exportPolicies().subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Policies_Portfolio_${new Date().toISOString().slice(0,10)}.xlsx`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastService.success('Policies exported to Excel successfully.');
      },
      error: (err) => {
        this.toastService.error('Export failed.');
      }
    });
  }

  openImportModal(): void {
    this.selectedFile = null;
    this.showImportModal = true;
  }

  closeImportModal(): void {
    this.showImportModal = false;
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  uploadExcelFile(): void {
    if (!this.selectedFile) return;
    this.importing = true;
    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.http.post<{ message: string, importedCount: number }>(`${environment.apiUrl}/api/excel/policies/import`, formData).subscribe({
      next: (res) => {
        this.importing = false;
        this.showImportModal = false;
        this.toastService.success(res.message || 'Excel policies imported successfully!');
        this.loadPolicies();
      },
      error: (err) => {
        this.importing = false;
        this.toastService.error(err?.error?.message || 'Import failed. Please check your Excel format.');
      }
    });
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

  canRenew(p: PolicyDto): boolean {
    const s = (p.status || '').toLowerCase().replace(/\s+/g, '');
    return s === 'expiringsoon';
  }

  canCancel(p: PolicyDto): boolean {
    const s = (p.status || '').toLowerCase().replace(/\s+/g, '');
    return s !== 'cancelled' && s !== 'expired';
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value || 0);
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }
}
