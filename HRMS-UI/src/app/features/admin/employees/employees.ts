import { Component, OnInit, signal, inject } from '@angular/core';

import { EmployeeService } from '../../../core/services/employee.service';
import { Employee } from '../../../core/models/employee.model';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { DepartmentService } from '../../../core/services/department.service';

import { Department } from '../../../core/models/department.model';

import { Manager } from '../../../core/models/manager.model';

import { AddEmployee } from '../../../core/models/add-employee.model';

import { CommonModule } from '@angular/common';

import { EmployeeProfile } from '../../../core/models/employee-profile.model';

import { UpdateEmployee } from '../../../core/models/update-employee.model';

import { ToastrService } from 'ngx-toastr';

import { LucideX, LucideLoader } from '@lucide/angular';

@Component({
  selector: 'app-employees',
  standalone: true,
  templateUrl: './employees.html',
  styleUrl: './employees.css',
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideX,
    LucideLoader
  ]
})
export class Employees implements OnInit {

  readonly employees = signal<Employee[]>([]);

  readonly loading = signal(false);

  readonly submitting = signal(false);

  readonly search = signal('');

  readonly pageNumber = signal(1);

  private toastr = inject(ToastrService);

  readonly pageSize = signal(10);

  readonly totalRecords = signal(0);

  readonly totalPages = signal(0);

  readonly showAddModal = signal(false);

  readonly departments = signal<Department[]>([]);

  readonly managers = signal<Manager[]>([]);

  createdEmployeeEmail = '';

  temporaryPassword = '';

  readonly showProfileDrawer = signal(false);

  readonly selectedProfile = signal<EmployeeProfile | null>(null);

  readonly showEditModal = signal(false);

  editingEmployeeId = '';

  employeeForm;

  editEmployeeForm; 

  readonly selectedFile = signal<File | null>(null);

  constructor(
    private employeeService: EmployeeService,
    private departmentService: DepartmentService,
    private fb: FormBuilder
  ) {

    this.employeeForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      designation: ['', Validators.required],
      departmentId: ['', Validators.required],
      role: ['Employee', Validators.required],
      managerId: [''],
      salary: [0, Validators.required]
    });

    this.editEmployeeForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phone: [''],
      designation: ['', Validators.required],
      salary: [0, Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadDepartments();
    this.loadManagers();
  }

  loadEmployees(): void {
    console.log('loadEmployees called');
    this.loading.set(true);

    this.employeeService
      .getEmployees(this.pageNumber(), this.pageSize(), this.search())
      .subscribe({
        next: (response) => {
          this.employees.set(response.data);
          this.totalRecords.set(response.totalRecords);
          this.totalPages.set(response.totalPages);
          this.loading.set(false);
        },
        error: (error) => {
          console.error(error);
          this.loading.set(false);
        }
      });
  }

  searchEmployees(): void {
    this.pageNumber.set(1);
    this.loadEmployees();
  }

  previousPage(): void {
    if (this.pageNumber() <= 1) {
      return;
    }
    this.pageNumber.update(p => p - 1);
    this.loadEmployees();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) {
      return;
    }
    this.pageNumber.update(p => p + 1);
    this.loadEmployees();
  }

  loadDepartments(): void {
    this.departmentService
      .getDepartments()
      .subscribe({
        next: response => {
          this.departments.set(response);
        }
      });
  }

  loadManagers(): void {
    this.employeeService
      .getManagers()
      .subscribe({
        next: response => {
          this.managers.set(response);
        }
      });
  }

  openAddEmployee(): void {
    this.showAddModal.set(true);
    this.employeeForm.reset({
      role: 'Employee',
      salary: 0
    });
  }

  closeAddEmployee(): void {
    this.showAddModal.set(false);
  }

  createEmployee(): void {
    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();
      return;
    }

    const request = this.employeeForm.getRawValue() as AddEmployee;
    this.submitting.set(true);

    this.employeeService
      .createEmployee(request)
      .subscribe({
        next: response => {
          this.createdEmployeeEmail = response.email;
          this.temporaryPassword = response.temporaryPassword;

          this.toastr.success(
            `Email: ${response.email} | Password: ${response.temporaryPassword}`,
            'Employee Created Successfully',
            { disableTimeOut: true, closeButton: true }
          );

          this.closeAddEmployee();
          this.loadEmployees();
          this.submitting.set(false);
        },
        error: error => {
          console.error(error);
          this.submitting.set(false);
        }
      });
  }

  viewEmployee(employeeId: string): void {
    this.employeeService
      .getEmployeeProfile(employeeId)
      .subscribe({
        next: (response) => {
          this.selectedProfile.set(response);
          this.showProfileDrawer.set(true);
        },
        error: (error) => {
          console.error(error);
        }
      });
  }

  closeProfileDrawer(): void {
    this.showProfileDrawer.set(false);
    this.selectedProfile.set(null);
  }

  openEditEmployee(employee: Employee): void {
    this.editingEmployeeId = employee.id;

    this.editEmployeeForm.patchValue({
      firstName: employee.firstName,
      lastName: employee.lastName,
      phone: employee.phone,
      designation: employee.designation,
      salary: employee.salary
    });

    this.showEditModal.set(true);
  }

  closeEditEmployee(): void {
    this.showEditModal.set(false);
    this.editingEmployeeId = '';
  }

  updateEmployee(): void {
    if (this.editEmployeeForm.invalid) {
      return;
    }

    const request = this.editEmployeeForm.getRawValue() as UpdateEmployee;
    this.submitting.set(true);

    this.employeeService
      .updateEmployee(this.editingEmployeeId, request)
      .subscribe({
        next: () => {
          this.toastr.success('Employee updated successfully');
          this.closeEditEmployee();
          this.loadEmployees();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
  }

  updateStatus(employeeId: string, event: Event): void {
    const status = (event.target as HTMLSelectElement).value;

    this.employeeService
      .updateEmployeeStatus(employeeId, status)
      .subscribe({
        next: () => {
          this.toastr.success('Status updated successfully');
        },
        error: (error) => {
          console.error(error);
          this.loadEmployees();
        }
      });
  }

  exportEmployees(): void {
    this.employeeService.exportEmployees().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'employees.xlsx';
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Employees list exported successfully');
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    this.selectedFile.set(input.files[0]);
  }

  importEmployees(): void {
    const file = this.selectedFile();
    if (!file) {
      this.toastr.warning('Select a file first');
      return;
    }
    this.submitting.set(true);

    this.employeeService.importEmployees(file).subscribe({
      next: () => {
        this.toastr.success('Import completed');
        this.loadEmployees();
        this.selectedFile.set(null);
        this.submitting.set(false);
      },
      error: (error) => {
        console.error(error);
        this.submitting.set(false);
      }
    });
  }
}
