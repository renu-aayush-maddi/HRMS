import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { EmployeeSalaryService } from '../../../../core/services/employee-salary.service';
import { SalaryStructureService } from '../../../../core/services/salary-structure.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { EmployeeSalaryResponse, SalaryHistoryResponse } from '../../../../core/models/employee-salary.model';
import { SalaryStructureResponse } from '../../../../core/models/salary-structure.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideAlertTriangle,
  LucideXCircle,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-employee-salary',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideAlertTriangle,
    LucideXCircle,
    LucideLoader
  ],
  templateUrl: './salary.html',
  styleUrl: './salary.css'
})
export class Salary implements OnInit {
  readonly employeeId = signal('');
  readonly activeSalary = signal<EmployeeSalaryResponse | null>(null);
  readonly history = signal<SalaryHistoryResponse[]>([]);
  readonly structures = signal<SalaryStructureResponse[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  // Assigner panel
  readonly showAssignForm = signal(false);
  assignForm;
  
  private toastr = inject(ToastrService);

  constructor(
    public employeeStore: EmployeeDetailsStore,
    private salaryService: EmployeeSalaryService,
    private structService: SalaryStructureService,
    private fb: FormBuilder
  ) {
    // React to selected employee changes from parent dropdown
    effect(() => {
      const id = this.employeeStore.selectedEmployeeId();
      if (id) {
        this.employeeId.set(id);
        this.loadSalaryDetails();
      } else {
        this.employeeId.set('');
        this.activeSalary.set(null);
        this.history.set([]);
      }
    });

    const currentYearStr = new Date().toISOString().slice(0, 10);
    this.assignForm = this.fb.group({
      salaryStructureId: ['', [Validators.required]],
      annualCtc: [0, [Validators.required, Validators.min(1000)]],
      effectiveFrom: [currentYearStr, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadStructures();
  }

  loadStructures(): void {
    this.structService.getAll().subscribe({
      next: (res) => {
        this.structures.set(res);
      },
      error: (err) => console.error('Failed to load salary structures', err)
    });
  }

  loadSalaryDetails(): void {
    if (!this.employeeId()) return;
    this.loading.set(true);
    
    // Get active salary
    this.salaryService.getActiveSalary(this.employeeId()).subscribe({
      next: (res) => {
        this.activeSalary.set(res);
        this.loadHistory();
      },
      error: (err) => {
        console.warn('Failed to load active salary details', err);
        this.activeSalary.set(null);
        this.loadHistory();
      }
    });
  }

  loadHistory(): void {
    this.salaryService.getSalaryHistory(this.employeeId()).subscribe({
      next: (res) => {
        this.history.set(res.sort((a, b) => b.effectiveFrom.localeCompare(a.effectiveFrom)));
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load salary history', err);
        this.history.set([]);
        this.loading.set(false);
      }
    });
  }

  toggleAssignForm(): void {
    this.showAssignForm.set(!this.showAssignForm());
    if (this.showAssignForm()) {
      const currentYearStr = new Date().toISOString().slice(0, 10);
      this.assignForm.reset({
        salaryStructureId: this.activeSalary()?.salaryStructureName ? '' : '',
        annualCtc: this.activeSalary()?.annualCtc || 30000,
        effectiveFrom: currentYearStr
      });
    }
  }

  submitAssignment(): void {
    if (this.assignForm.invalid) {
      this.assignForm.markAllAsTouched();
      return;
    }

    const val = this.assignForm.value;
    const payload = {
      employeeId: this.employeeId(),
      salaryStructureId: val.salaryStructureId || '',
      annualCtc: val.annualCtc || 0,
      effectiveFrom: val.effectiveFrom || ''
    };

    this.submitting.set(true);
    this.salaryService.assignSalary(payload).subscribe({
      next: () => {
        this.toastr.success('Salary profile assigned successfully.');
        this.showAssignForm.set(false);
        this.loadSalaryDetails();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }
}
