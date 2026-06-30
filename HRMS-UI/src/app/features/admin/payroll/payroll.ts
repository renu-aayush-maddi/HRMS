import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { PayrollService } from '../../../core/services/payroll.service';
import { PayrollResponse, PayrollFilter } from '../../../core/models/payroll.model';
import { ToastrService } from 'ngx-toastr';

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
    LucideLoader
  ],
  templateUrl: './payroll.html',
  styleUrl: './payroll.css',
})
export class Payroll implements OnInit {
  readonly payrolls = signal<PayrollResponse[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Filters
  readonly filterMonth = signal(0);
  readonly filterYear = signal(0);
  readonly filterStatus = signal('');

  // Generate Modal
  readonly showGenerateModal = signal(false);
  generateForm;
  
  private toastr = inject(ToastrService);

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
    this.loadPayrolls();
  }

  loadPayrolls(): void {
    this.loading.set(true);
    const filter: PayrollFilter = {};
    if (this.filterMonth() > 0) filter.payMonth = this.filterMonth();
    if (this.filterYear() > 0) filter.payYear = this.filterYear();
    if (this.filterStatus()) filter.status = this.filterStatus();

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

  applyFilters(): void {
    this.loadPayrolls();
  }

  resetFilters(): void {
    this.filterMonth.set(0);
    this.filterYear.set(0);
    this.filterStatus.set('');
    this.loadPayrolls();
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
    if (this.filterMonth() > 0) filter.payMonth = this.filterMonth();
    if (this.filterYear() > 0) filter.payYear = this.filterYear();
    if (this.filterStatus()) filter.status = this.filterStatus();

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
