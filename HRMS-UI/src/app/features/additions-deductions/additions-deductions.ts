import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { BonusDeductionService } from '../../core/services/bonus-deduction.service';
import { EmployeeService } from '../../core/services/employee.service';
import { 
  BonusResponse, BonusFilter, 
  DeductionResponse, DeductionFilter 
} from '../../core/models/bonus-deduction.model';
import { Employee } from '../../core/models/employee.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideDownload,
  LucidePlus,
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-additions-deductions',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideDownload,
    LucidePlus,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideLoader
  ],
  templateUrl: './additions-deductions.html',
  styleUrl: './additions-deductions.css'
})
export class AdditionsDeductions implements OnInit {
  readonly activeTab = signal<'bonuses' | 'deductions'>('bonuses');
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly employees = signal<Employee[]>([]);
  
  private toastr = inject(ToastrService);

  // Month list helper
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

  // Bonuses state
  readonly bonuses = signal<BonusResponse[]>([]);
  readonly bonusPage = signal(1);
  readonly bonusPageSize = signal(10);
  readonly bonusTotalPages = signal(0);
  readonly bonusTotalRecords = signal(0);
  readonly bonusStatus = signal('');
  readonly bonusEmployeeId = signal('');
  readonly bonusMonth = signal<number | undefined>(undefined);
  readonly bonusYear = signal<number | undefined>(undefined);

  // Deductions state
  readonly deductions = signal<DeductionResponse[]>([]);
  readonly deductionPage = signal(1);
  readonly deductionPageSize = signal(10);
  readonly deductionTotalPages = signal(0);
  readonly deductionTotalRecords = signal(0);
  readonly deductionStatus = signal('');
  readonly deductionEmployeeId = signal('');
  readonly deductionMonth = signal<number | undefined>(undefined);
  readonly deductionYear = signal<number | undefined>(undefined);

  // Add Request Modal
  readonly showModal = signal(false);
  requestForm;

  constructor(
    private service: BonusDeductionService,
    private employeeService: EmployeeService,
    private fb: FormBuilder
  ) {
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth() + 1;

    this.requestForm = this.fb.group({
      employeeId: ['', [Validators.required]],
      amount: [0, [Validators.required, Validators.min(1)]],
      reason: ['', [Validators.required, Validators.maxLength(300)]],
      month: [currentMonth, [Validators.required, Validators.min(1), Validators.max(12)]],
      year: [currentYear, [Validators.required, Validators.min(2000), Validators.max(2100)]]
    });
  }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadData();
  }

  loadEmployees(): void {
    this.employeeService.getEmployeeLookup().subscribe({
      next: (res: any) => {
        this.employees.set(res.data || []);
      },
      error: (err: any) => console.error(err)
    });
  }

  setTab(tab: 'bonuses' | 'deductions'): void {
    this.activeTab.set(tab);
    this.loadData();
  }

  loadData(): void {
    if (this.activeTab() === 'bonuses') {
      this.loadBonuses();
    } else {
      this.loadDeductions();
    }
  }

  loadBonuses(): void {
    this.loading.set(true);
    const filter: BonusFilter = {
      pageNumber: this.bonusPage(),
      pageSize: this.bonusPageSize(),
      descending: true
    };

    if (this.bonusStatus()) filter.status = this.bonusStatus();
    if (this.bonusEmployeeId()) filter.employeeId = this.bonusEmployeeId();
    if (this.bonusMonth() !== undefined && this.bonusMonth() !== null) filter.bonusMonth = Number(this.bonusMonth());
    if (this.bonusYear() !== undefined && this.bonusYear() !== null) filter.bonusYear = Number(this.bonusYear());

    this.service.getBonuses(filter).subscribe({
      next: (res) => {
        this.bonuses.set(res.data);
        this.bonusTotalPages.set(res.totalPages);
        this.bonusTotalRecords.set(res.totalRecords);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  loadDeductions(): void {
    this.loading.set(true);
    const filter: DeductionFilter = {
      pageNumber: this.deductionPage(),
      pageSize: this.deductionPageSize(),
      descending: true
    };

    if (this.deductionStatus()) filter.status = this.deductionStatus();
    if (this.deductionEmployeeId()) filter.employeeId = this.deductionEmployeeId();
    if (this.deductionMonth() !== undefined && this.deductionMonth() !== null) filter.deductionMonth = Number(this.deductionMonth());
    if (this.deductionYear() !== undefined && this.deductionYear() !== null) filter.deductionYear = Number(this.deductionYear());

    this.service.getDeductions(filter).subscribe({
      next: (res) => {
        this.deductions.set(res.data);
        this.deductionTotalPages.set(res.totalPages);
        this.deductionTotalRecords.set(res.totalRecords);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    if (this.activeTab() === 'bonuses') {
      this.bonusPage.set(1);
      this.loadBonuses();
    } else {
      this.deductionPage.set(1);
      this.loadDeductions();
    }
  }

  onEmployeeFilterChange(value: string): void {
    if (this.activeTab() === 'bonuses') {
      this.bonusEmployeeId.set(value);
    } else {
      this.deductionEmployeeId.set(value);
    }
    this.applyFilters();
  }

  onStatusFilterChange(value: string): void {
    if (this.activeTab() === 'bonuses') {
      this.bonusStatus.set(value);
    } else {
      this.deductionStatus.set(value);
    }
    this.applyFilters();
  }

  onMonthFilterChange(value: any): void {
    const val = value ? Number(value) : undefined;
    if (this.activeTab() === 'bonuses') {
      this.bonusMonth.set(val);
    } else {
      this.deductionMonth.set(val);
    }
    this.applyFilters();
  }

  onYearFilterChange(value: any): void {
    const val = value ? Number(value) : undefined;
    if (this.activeTab() === 'bonuses') {
      this.bonusYear.set(val);
    } else {
      this.deductionYear.set(val);
    }
    this.applyFilters();
  }

  resetFilters(): void {
    if (this.activeTab() === 'bonuses') {
      this.bonusStatus.set('');
      this.bonusEmployeeId.set('');
      this.bonusMonth.set(undefined);
      this.bonusYear.set(undefined);
      this.bonusPage.set(1);
      this.loadBonuses();
    } else {
      this.deductionStatus.set('');
      this.deductionEmployeeId.set('');
      this.deductionMonth.set(undefined);
      this.deductionYear.set(undefined);
      this.deductionPage.set(1);
      this.loadDeductions();
    }
  }

  previousPage(): void {
    if (this.activeTab() === 'bonuses') {
      if (this.bonusPage() <= 1) return;
      this.bonusPage.update(p => p - 1);
      this.loadBonuses();
    } else {
      if (this.deductionPage() <= 1) return;
      this.deductionPage.update(p => p - 1);
      this.loadDeductions();
    }
  }

  nextPage(): void {
    if (this.activeTab() === 'bonuses') {
      if (this.bonusPage() >= this.bonusTotalPages()) return;
      this.bonusPage.update(p => p + 1);
      this.loadBonuses();
    } else {
      if (this.deductionPage() >= this.deductionTotalPages()) return;
      this.deductionPage.update(p => p + 1);
      this.loadDeductions();
    }
  }

  openAddModal(): void {
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth() + 1;

    this.requestForm.reset({
      employeeId: '',
      amount: 0,
      reason: '',
      month: currentMonth,
      year: currentYear
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.requestForm.reset();
  }

  submitRequest(): void {
    if (this.requestForm.invalid) {
      this.requestForm.markAllAsTouched();
      return;
    }

    const val = this.requestForm.value;
    const payload = {
      employeeId: val.employeeId || '',
      amount: Number(val.amount) || 0,
      reason: val.reason || '',
      month: Number(val.month) || 1,
      year: Number(val.year) || 2026
    };

    this.submitting.set(true);
    if (this.activeTab() === 'bonuses') {
      this.service.createBonus({
        employeeId: payload.employeeId,
        amount: payload.amount,
        reason: payload.reason,
        bonusMonth: payload.month,
        bonusYear: payload.year
      }).subscribe({
        next: () => {
          this.toastr.success('Bonus addition created successfully.');
          this.closeModal();
          this.loadBonuses();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.service.createDeduction({
        employeeId: payload.employeeId,
        amount: payload.amount,
        reason: payload.reason,
        deductionMonth: payload.month,
        deductionYear: payload.year
      }).subscribe({
        next: () => {
          this.toastr.success('Deduction created successfully.');
          this.closeModal();
          this.loadDeductions();
          this.submitting.set(false);
        },
        error: (err: any) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  approveRequest(id: string): void {
    if (confirm('Approve this payroll adjustment?')) {
      this.submitting.set(true);
      if (this.activeTab() === 'bonuses') {
        this.service.approveBonus(id).subscribe({
          next: () => {
            this.toastr.success('Approved successfully.');
            this.loadBonuses();
            this.submitting.set(false);
          },
          error: (err) => {
            console.error(err);
            this.submitting.set(false);
          }
        });
      } else {
        this.service.approveDeduction(id).subscribe({
          next: () => {
            this.toastr.success('Approved successfully.');
            this.loadDeductions();
            this.submitting.set(false);
          },
          error: (err) => {
            console.error(err);
            this.submitting.set(false);
          }
        });
      }
    }
  }

  rejectRequest(id: string): void {
    if (confirm('Reject this payroll adjustment?')) {
      this.submitting.set(true);
      if (this.activeTab() === 'bonuses') {
        this.service.rejectBonus(id).subscribe({
          next: () => {
            this.toastr.success('Rejected successfully.');
            this.loadBonuses();
            this.submitting.set(false);
          },
          error: (err) => {
            console.error(err);
            this.submitting.set(false);
          }
        });
      } else {
        this.service.rejectDeduction(id).subscribe({
          next: () => {
            this.toastr.success('Rejected successfully.');
            this.loadDeductions();
            this.submitting.set(false);
          },
          error: (err) => {
            console.error(err);
            this.submitting.set(false);
          }
        });
      }
    }
  }

  exportReport(): void {
    if (this.activeTab() === 'bonuses') {
      const filter: BonusFilter = {};
      if (this.bonusStatus()) filter.status = this.bonusStatus();
      if (this.bonusEmployeeId()) filter.employeeId = this.bonusEmployeeId();
      if (this.bonusMonth() !== undefined) filter.bonusMonth = Number(this.bonusMonth());
      if (this.bonusYear() !== undefined) filter.bonusYear = Number(this.bonusYear());

      this.service.exportBonuses(filter).subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = `bonuses_export_${new Date().toISOString().slice(0, 10)}.xlsx`;
          a.click();
          window.URL.revokeObjectURL(url);
          this.toastr.success('Bonuses list exported successfully.');
        },
        error: (err) => {
          console.error(err);
        }
      });
    } else {
      const filter: DeductionFilter = {};
      if (this.deductionStatus()) filter.status = this.deductionStatus();
      if (this.deductionEmployeeId()) filter.employeeId = this.deductionEmployeeId();
      if (this.deductionMonth() !== undefined) filter.deductionMonth = Number(this.deductionMonth());
      if (this.deductionYear() !== undefined) filter.deductionYear = Number(this.deductionYear());

      this.service.exportDeductions(filter).subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = `deductions_export_${new Date().toISOString().slice(0, 10)}.xlsx`;
          a.click();
          window.URL.revokeObjectURL(url);
          this.toastr.success('Deductions list exported successfully.');
        },
        error: (err) => {
          console.error(err);
        }
      });
    }
  }
}
