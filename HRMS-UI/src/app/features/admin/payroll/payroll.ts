import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { PayrollService } from '../../../core/services/payroll.service';
import { PayrollResponse, PayrollFilter } from '../../../core/models/payroll.model';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterBarComponent } from '../../../shared/components/filter-bar/filter-bar';
import { FilterField, SortOption } from '../../../shared/components/filter-bar/filter-bar.model';

import {
  LucideDownload,
  LucideCoins,
  LucideCheck,
  LucideCreditCard,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-payroll',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideDownload,
    LucideCoins,
    LucideCheck,
    LucideCreditCard,
    LucideX,
    LucideLoader,
    FilterBarComponent
  ],
  templateUrl: './payroll.html',
  styleUrl: './payroll.css',
})
export class Payroll implements OnInit {
  readonly payrolls = signal<PayrollResponse[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Reusable Filter Config & State
  filterFields: FilterField[] = [
    { key: 'employeeId', label: 'Employee ID', type: 'text', placeholder: 'Search Employee ID...' },
    { key: 'payMonth', label: 'Pay Month', type: 'select', options: [
      { value: 1, label: 'January' },
      { value: 2, label: 'February' },
      { value: 3, label: 'March' },
      { value: 4, label: 'April' },
      { value: 5, label: 'May' },
      { value: 6, label: 'June' },
      { value: 7, label: 'July' },
      { value: 8, label: 'August' },
      { value: 9, label: 'September' },
      { value: 10, label: 'October' },
      { value: 11, label: 'November' },
      { value: 12, label: 'December' }
    ]},
    { key: 'payYear', label: 'Pay Year', type: 'number', placeholder: 'Enter year (e.g. 2026)...' },
    { key: 'status', label: 'Status', type: 'select', options: [
      { value: 'Draft', label: 'Draft' },
      { value: 'Approved', label: 'Approved' },
      { value: 'Paid', label: 'Paid' }
    ]},
    { key: 'minNetSalary', label: 'Min Net Salary', type: 'number', placeholder: 'Min salary...' },
    { key: 'maxNetSalary', label: 'Max Net Salary', type: 'number', placeholder: 'Max salary...' }
  ];

  sortOptions: SortOption[] = [
    { value: 'PayMonth', label: 'Pay Month' },
    { value: 'PayYear', label: 'Pay Year' },
    { value: 'NetSalary', label: 'Net Salary' },
    { value: 'Status', label: 'Status' }
  ];

  filters: { [key: string]: any } = {};
  sortBy = 'PayMonth';
  descending = false;

  // Generate Modal
  readonly showGenerateModal = signal(false);
  generateForm;
  
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  months = [
    { value: 1, name: 'January' },
    { value: 2, name: 'February' },
    { value: 3, name: 'March' },
    { value: 4, name: 'April' },
    { value: 5, name: 'May' },
    { value: 6, name: 'June' },
    { value: 7, name: 'July' },
    { value: 8, name: 'August' },
    { value: 9, name: 'September' },
    { value: 10, name: 'October' },
    { value: 11, name: 'November' },
    { value: 12, name: 'December' }
  ];

  years = [2024, 2025, 2026, 2027, 2028];

  constructor(
    private payrollService: PayrollService,
    private fb: FormBuilder
  ) {
    const currentMonth = new Date().getMonth() + 1;
    const currentYear = new Date().getFullYear();

    this.generateForm = this.fb.group({
      payMonth: [currentMonth, [Validators.required, Validators.min(1), Validators.max(12)]],
      payYear: [currentYear, [Validators.required, Validators.min(2024), Validators.max(2028)]]
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const newFilters: any = {};
      if (params['employeeId']) newFilters['employeeId'] = params['employeeId'];
      if (params['payMonth']) newFilters['payMonth'] = parseInt(params['payMonth'], 10);
      if (params['payYear']) newFilters['payYear'] = parseInt(params['payYear'], 10);
      if (params['status']) newFilters['status'] = params['status'];
      if (params['minNetSalary']) newFilters['minNetSalary'] = parseFloat(params['minNetSalary']);
      if (params['maxNetSalary']) newFilters['maxNetSalary'] = parseFloat(params['maxNetSalary']);

      this.filters = newFilters;
      this.sortBy = params['sortBy'] || 'PayMonth';
      this.descending = params['descending'] === 'true';

      this.loadPayrolls();
    });
  }

  loadPayrolls(): void {
    this.loading.set(true);
    const filter: PayrollFilter = {
      sortBy: this.sortBy,
      descending: this.descending
    };
    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['payMonth']) filter.payMonth = this.filters['payMonth'];
    if (this.filters['payYear']) filter.payYear = this.filters['payYear'];
    if (this.filters['status']) filter.status = this.filters['status'];
    if (this.filters['minNetSalary']) filter.minNetSalary = this.filters['minNetSalary'];
    if (this.filters['maxNetSalary']) filter.maxNetSalary = this.filters['maxNetSalary'];

    this.payrollService.getPayrolls(filter).subscribe({
      next: (data: any) => {
        const items = data?.data || (Array.isArray(data) ? data : []);
        this.payrolls.set(items);
        this.loading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.loading.set(false);
      }
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

  openGenerateModal(): void {
    const currentMonth = new Date().getMonth() + 1;
    const currentYear = new Date().getFullYear();
    this.generateForm.reset({
      payMonth: currentMonth,
      payYear: currentYear
    });
    this.showGenerateModal.set(true);
  }

  closeGenerateModal(): void {
    this.showGenerateModal.set(false);
    this.generateForm.reset();
  }

  submitGenerate(): void {
    if (this.generateForm.invalid) return;

    this.submitting.set(true);
    const dto = {
      payMonth: Number(this.generateForm.value.payMonth!),
      payYear: Number(this.generateForm.value.payYear!)
    };

    this.payrollService.generateMonthlyPayroll(dto).subscribe({
      next: (res) => {
        this.toastr.success(res?.message || 'Monthly payroll generated successfully.');
        this.closeGenerateModal();
        this.loadPayrolls();
        this.submitting.set(false);
      },
      error: (error) => {
        console.error(error);
        this.submitting.set(false);
      }
    });
  }

  approvePayroll(id: string): void {
    if (confirm('Are you sure you want to approve this payroll record?')) {
      this.payrollService.approvePayroll(id).subscribe({
        next: () => {
          this.toastr.success('Payroll record approved.');
          this.loadPayrolls();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }

  markPaid(id: string): void {
    if (confirm('Are you sure you want to mark this payroll record as paid?')) {
      this.payrollService.markPayrollPaid(id).subscribe({
        next: () => {
          this.toastr.success('Payroll record marked as Paid.');
          this.loadPayrolls();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }

  exportPayrolls(): void {
    const filter: PayrollFilter = {};
    if (this.filters['employeeId']) filter.employeeId = this.filters['employeeId'];
    if (this.filters['payMonth']) filter.payMonth = this.filters['payMonth'];
    if (this.filters['payYear']) filter.payYear = this.filters['payYear'];
    if (this.filters['status']) filter.status = this.filters['status'];
    if (this.filters['minNetSalary']) filter.minNetSalary = this.filters['minNetSalary'];
    if (this.filters['maxNetSalary']) filter.maxNetSalary = this.filters['maxNetSalary'];

    this.payrollService.exportPayrolls(filter).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `payrolls_export_${new Date().toISOString().slice(0, 10)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Payrolls list exported successfully.');
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  getMonthName(monthNum: number): string {
    const found = this.months.find(m => m.value === monthNum);
    return found ? found.name : monthNum.toString();
  }
}
