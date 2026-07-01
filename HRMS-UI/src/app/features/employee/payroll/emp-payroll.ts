import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeSelfService } from '../../../core/services/employee-self.service';
import { PayrollResponse, PayrollFilter } from '../../../core/models/payroll.model';
import { BonusResponse, DeductionResponse } from '../../../core/models/bonus-deduction.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

type ActiveTab = 'payroll' | 'bonuses' | 'deductions';

import {
  LucideCheckCircle,
  LucideAlertTriangle,
  LucideCoins,
  LucideGift,
  LucideTrendingDown,
  LucideDownload
} from '@lucide/angular';

@Component({
  selector: 'app-emp-payroll',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideCheckCircle,
    LucideAlertTriangle,
    LucideCoins,
    LucideGift,
    LucideTrendingDown,
    LucideDownload,
    FilterBarComponent
  ],
  templateUrl: './emp-payroll.html',
  styleUrl: './emp-payroll.css',
})
export class EmpPayroll implements OnInit {
  readonly activeTab = signal<ActiveTab>('payroll');

  // Payroll
  readonly payrolls = signal<PayrollResponse[]>([]);
  readonly payrollsLoading = signal(false);

  // Bonuses
  readonly bonuses = signal<BonusResponse[]>([]);
  readonly bonusesLoading = signal(false);
  readonly bonusTotalAmount = signal(0);

  // Deductions
  readonly deductions = signal<DeductionResponse[]>([]);
  readonly deductionsLoading = signal(false);
  readonly deductionTotalAmount = signal(0);

  // Download
  readonly downloadingId = signal<string | null>(null);

  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Reusable Filter bar config & state
  filterFields: FilterField[] = [
    { key: 'payYear', label: 'Pay Year', type: 'number', placeholder: 'Enter year (e.g. 2026)...' }
  ];

  sortOptions: SortOption[] = [
    { value: 'PayMonth', label: 'Pay Month' },
    { value: 'PayYear', label: 'Pay Year' },
    { value: 'NetSalary', label: 'Net Salary' }
  ];

  filters: { [key: string]: any } = {};
  sortBy = 'PayMonth';
  descending = false;

  constructor(private empService: EmployeeSelfService) {}

  ngOnInit(): void {
    this.loadBonuses();
    this.loadDeductions();

    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['payYear']) newFilters['payYear'] = parseInt(params['payYear'], 10);

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'PayMonth';
      this.descending = params['descending'] === 'true';

      this.loadPayrolls();
    });
  }

  setTab(tab: ActiveTab): void {
    this.activeTab.set(tab);
  }

  loadPayrolls(): void {
    this.payrollsLoading.set(true);
    const filter: PayrollFilter = {
      sortBy: this.sortBy,
      descending: this.descending
    };
    if (this.filters['payYear']) filter.payYear = this.filters['payYear'];

    this.empService.getMyPayrolls(filter).subscribe({
      next: (data: any) => {
        const items = data?.data || (Array.isArray(data) ? data : []);
        this.payrolls.set(items);
        this.payrollsLoading.set(false);
      },
      error: () => { this.payrollsLoading.set(false); }
    });
  }

  onFiltersChanged(updatedFilters: any): void {
    const queryParams: any = { ...updatedFilters };
    Object.keys(queryParams).forEach(k => {
      if (queryParams[k] === null || queryParams[k] === undefined || queryParams[k] === '') {
        delete queryParams[k];
      }
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'merge'
    });
  }

  onSortChanged(event: { sortBy: string; descending: boolean }): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        sortBy: event.sortBy,
        descending: event.descending.toString()
      },
      queryParamsHandling: 'merge'
    });
  }

  loadBonuses(): void {
    this.bonusesLoading.set(true);
    this.empService.getMyBonuses({ pageSize: 50 }).subscribe({
      next: (result) => {
        this.bonuses.set(result.data ?? []);
        this.bonusTotalAmount.set(this.bonuses().reduce((s, b) => s + b.amount, 0));
        this.bonusesLoading.set(false);
      },
      error: () => { this.bonusesLoading.set(false); }
    });
  }

  loadDeductions(): void {
    this.deductionsLoading.set(true);
    this.empService.getMyDeductions({ pageSize: 50 }).subscribe({
      next: (result) => {
        this.deductions.set(result.data ?? []);
        this.deductionTotalAmount.set(this.deductions().reduce((s, d) => s + d.amount, 0));
        this.deductionsLoading.set(false);
      },
      error: () => { this.deductionsLoading.set(false); }
    });
  }

  downloadPayslip(payrollId: string): void {
    this.downloadingId.set(payrollId);
    this.empService.downloadMyPayslip(payrollId).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Payslip-${payrollId}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.downloadingId.set(null);
        this.notify('success', 'Payslip downloaded successfully!');
      },
      error: () => {
        this.notify('error', 'Failed to download payslip. Please try again.');
        this.downloadingId.set(null);
      }
    });
  }

  getMonthName(month: number): string {
    return new Date(2000, month - 1, 1).toLocaleString('default', { month: 'long' });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Generated': 'badge-info',
      'Approved': 'badge-success',
      'Paid': 'badge-green',
      'Pending': 'badge-warning',
    };
    return map[status] || 'badge-secondary';
  }

  getBonusStatusClass(status: string): string {
    return status === 'Approved' ? 'badge-success' : status === 'Rejected' ? 'badge-danger' : 'badge-warning';
  }

  private notify(type: 'success' | 'error', msg: string): void {
    if (type === 'success') { this.successMessage.set(msg); setTimeout(() => this.successMessage.set(null), 4000); }
    else { this.errorMessage.set(msg); setTimeout(() => this.errorMessage.set(null), 4000); }
  }

  get currentYear(): number { return new Date().getFullYear(); }
  get years(): number[] {
    const y = this.currentYear;
    return [y, y - 1, y - 2, y - 3];
  }
}
