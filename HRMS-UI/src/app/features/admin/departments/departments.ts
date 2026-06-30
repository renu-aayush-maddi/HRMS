import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { DepartmentService } from '../../../core/services/department.service';
import { Department } from '../../../core/models/department.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucidePlus,
  LucideEdit,
  LucideTrash,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucidePlus,
    LucideEdit,
    LucideTrash,
    LucideX,
    LucideLoader
  ],
  templateUrl: './departments.html',
  styleUrl: './departments.css',
})
export class Departments implements OnInit {
  readonly departments = signal<Department[]>([]);
  readonly filteredDepartments = signal<Department[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);
  readonly searchQuery = signal('');

  readonly showModal = signal(false);
  editingId: string | null = null;
  departmentForm;

  private toastr = inject(ToastrService);

  constructor(
    private departmentService: DepartmentService,
    private fb: FormBuilder
  ) {
    this.departmentForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]]
    });
  }

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.loading.set(true);
    this.departmentService.getDepartments().subscribe({
      next: (data) => {
        this.departments.set(data);
        this.applyFilter();
        this.loading.set(false);
      },
      error: (error) => {
        console.error(error);
        this.loading.set(false);
      }
    });
  }

  applyFilter(): void {
    const query = this.searchQuery();
    if (!query) {
      this.filteredDepartments.set([...this.departments()]);
    } else {
      const q = query.toLowerCase();
      this.filteredDepartments.set(
        this.departments().filter(d => d.name.toLowerCase().includes(q))
      );
    }
  }

  openAddModal(): void {
    this.editingId = null;
    this.departmentForm.reset();
    this.showModal.set(true);
  }

  openEditModal(dept: Department): void {
    this.editingId = dept.id;
    this.departmentForm.patchValue({
      name: dept.name
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.departmentForm.reset();
  }

  saveDepartment(): void {
    if (this.departmentForm.invalid) {
      this.departmentForm.markAllAsTouched();
      return;
    }

    const name = this.departmentForm.value.name!;
    this.submitting.set(true);
    
    if (this.editingId) {
      // Update
      this.departmentService.updateDepartment(this.editingId, name).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Department updated successfully.');
          this.closeModal();
          this.loadDepartments();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    } else {
      // Add
      this.departmentService.createDepartment(name).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Department added successfully.');
          this.closeModal();
          this.loadDepartments();
          this.submitting.set(false);
        },
        error: (error) => {
          console.error(error);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteDepartment(id: string): void {
    if (confirm('Are you sure you want to delete this department?')) {
      this.departmentService.deleteDepartment(id).subscribe({
        next: (msg) => {
          this.toastr.success(msg || 'Department deleted successfully.');
          this.loadDepartments();
        },
        error: (error) => {
          console.error(error);
        }
      });
    }
  }
}
