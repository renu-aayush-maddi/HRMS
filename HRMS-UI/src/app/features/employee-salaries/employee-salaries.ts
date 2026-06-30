import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { EmployeeSalaryService } from '../../core/services/employee-salary.service';
import { SalaryStructureService } from '../../core/services/salary-structure.service';
import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeSalaryResponse } from '../../core/models/employee-salary.model';
import { SalaryStructureResponse } from '../../core/models/salary-structure.model';
import { ToastrService } from 'ngx-toastr';

import { 
  LucideSearch, 
  LucidePlus, 
  LucideX, 
  LucideLoader,
  LucideAlertTriangle
} from '@lucide/angular';

@Component({
  selector: 'app-employee-salaries',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ReactiveFormsModule,
    LucideSearch, 
    LucidePlus, 
    LucideX, 
    LucideLoader,
    LucideAlertTriangle
  ],
  templateUrl: './employee-salaries.html',
  styleUrl: './employee-salaries.css'
})
export class EmployeeSalaries implements OnInit {
  readonly salaries = signal<EmployeeSalaryResponse[]>([]);
  readonly filteredSalaries = signal<EmployeeSalaryResponse[]>([]);
  readonly structures = signal<SalaryStructureResponse[]>([]);
  readonly employees = signal<any[]>([]);
  
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  // Search & Filter
  readonly searchQuery = signal('');
  readonly structureFilter = signal('');

  // Assign Modal
  readonly showAssignModal = signal(false);
  readonly assigning = signal(false);
  readonly assignError = signal<string | null>(null);

  assignForm;

  private toastr = inject(ToastrService);

  constructor(
    private salaryService: EmployeeSalaryService,
    private structService: SalaryStructureService,
    private employeeService: EmployeeService,
    private fb: FormBuilder
  ) {
    const currentYearStr = new Date().toISOString().slice(0, 10);
    this.assignForm = this.fb.group({
      employeeId: ['', [Validators.required]],
      salaryStructureId: ['', [Validators.required]],
      annualCtc: [300000, [Validators.required, Validators.min(1000)]],
      effectiveFrom: [currentYearStr, [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadSalaries();
    this.loadStructures();
    this.loadEmployees();
  }

  loadSalaries(): void {
    this.loading.set(true);
    this.error.set(null);
    this.salaryService.getAll().subscribe({
      next: (res) => {
        this.salaries.set(res);
        this.applyFilters();
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load employee salaries', err);
        this.error.set('Failed to load salaries directory.');
        this.loading.set(false);
      }
    });
  }

  loadStructures(): void {
    this.structService.getAll().subscribe({
      next: (res) => this.structures.set(res),
      error: (err) => console.error('Failed to load structures', err)
    });
  }

  loadEmployees(): void {
    this.employeeService.getEmployeeLookup().subscribe({
      next: (res) => {
        const items = res?.data || (Array.isArray(res) ? res : []);
        this.employees.set(items);
      },
      error: (err) => console.error('Failed to load employees lookup', err)
    });
  }

  applyFilters(): void {
    let result = [...this.salaries()];

    if (this.searchQuery().trim()) {
      const q = this.searchQuery().toLowerCase();
      result = result.filter(s => 
        s.employeeName.toLowerCase().includes(q)
      );
    }

    if (this.structureFilter()) {
      result = result.filter(s => s.salaryStructureName === this.structureFilter());
    }

    this.filteredSalaries.set(result);
  }

  openAssignModal(salary?: EmployeeSalaryResponse): void {
    this.assignError.set(null);
    const currentYearStr = new Date().toISOString().slice(0, 10);
    
    if (salary) {
      const emp = this.employees().find(e => (e.firstName + ' ' + e.lastName) === salary.employeeName);
      const matchedStruct = this.structures().find(s => s.name === salary.salaryStructureName);

      this.assignForm.reset({
        employeeId: emp?.id || '',
        salaryStructureId: matchedStruct?.id || '',
        annualCtc: salary.annualCtc,
        effectiveFrom: salary.effectiveFrom || currentYearStr
      });
      this.assignForm.get('employeeId')?.disable();
    } else {
      this.assignForm.reset({
        employeeId: '',
        salaryStructureId: '',
        annualCtc: 300000,
        effectiveFrom: currentYearStr
      });
      this.assignForm.get('employeeId')?.enable();
    }

    this.showAssignModal.set(true);
  }

  closeAssignModal(): void {
    this.showAssignModal.set(false);
  }

  submitAssignment(): void {
    if (this.assignForm.invalid) {
      this.assignForm.markAllAsTouched();
      return;
    }

    this.assigning.set(true);
    this.assignError.set(null);

    const val = this.assignForm.getRawValue();
    const payload = {
      employeeId: val.employeeId || '',
      salaryStructureId: val.salaryStructureId || '',
      annualCtc: val.annualCtc || 0,
      effectiveFrom: val.effectiveFrom || ''
    };

    this.salaryService.assignSalary(payload).subscribe({
      next: () => {
        this.loadSalaries();
        this.closeAssignModal();
        this.assigning.set(false);
        this.toastr.success('Salary profile assigned successfully.');
      },
      error: (err) => {
        console.error('Failed to assign salary', err);
        this.assignError.set(err?.error?.message || 'Failed to assign salary profile.');
        this.assigning.set(false);
      }
    });
  }
}
