import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { EmployeeEducationService } from '../../../../core/services/employee-education.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { EmployeeEducation, AddEmployeeEducation } from '../../../../core/models/employee-subprofile.model';
import { ToastrService } from 'ngx-toastr';
import { AuthStore } from '../../../../stores/auth/auth.store';

import {
  LucideUpload,
  LucideDownload,
  LucidePlus,
  LucideEdit,
  LucideTrash2,
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-educations',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideUpload,
    LucideDownload,
    LucidePlus,
    LucideEdit,
    LucideTrash2,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideLoader
  ],
  templateUrl: './educations.html',
  styleUrl: './educations.css',
})
export class Educations implements OnInit {
  readonly educations = signal<EmployeeEducation[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);
  readonly totalPages = signal(0);
  readonly totalRecords = signal(0);
  
  private toastr = inject(ToastrService);
  private authStore = inject(AuthStore);

  isAdminOrHr(): boolean {
    const role = this.authStore.currentUser()?.role;
    return role === 'Admin' || role === 'HR';
  }

  readonly showModal = signal(false);
  editingId: string | null = null;
  educationForm;
  readonly selectedFile = signal<File | null>(null);

  constructor(
    private educationService: EmployeeEducationService,
    public employeeStore: EmployeeDetailsStore,
    private fb: FormBuilder
  ) {
    this.educationForm = this.fb.group({
      degree: ['', [Validators.required, Validators.maxLength(100)]],
      specialization: ['', [Validators.required, Validators.maxLength(100)]],
      institutionName: ['', [Validators.required, Validators.maxLength(150)]],
      graduationYear: [new Date().getFullYear(), [Validators.required, Validators.min(1950), Validators.max(new Date().getFullYear() + 5)]],
      percentage: [null as number | null, [Validators.min(0), Validators.max(100)]]
    });

    // Auto-reload on employee changes
    effect(() => {
      const employeeId = this.employeeStore.selectedEmployeeId();
      this.pageNumber.set(1);
      this.loadEducations();
    });
  }

  ngOnInit(): void {
    // Rely on effect for load
  }

  loadEducations(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.educations.set([]);
      this.totalPages.set(0);
      this.totalRecords.set(0);
      return;
    }

    this.loading.set(true);
    this.educationService.getEducations(employeeId, this.pageNumber(), this.pageSize()).subscribe({
      next: (response) => {
        this.educations.set(response.data);
        this.totalPages.set(response.totalPages);
        this.totalRecords.set(response.totalRecords);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  previousPage(): void {
    if (this.pageNumber() <= 1) return;
    this.pageNumber.update(p => p - 1);
    this.loadEducations();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.pageNumber.update(p => p + 1);
    this.loadEducations();
  }

  openAddModal(): void {
    this.editingId = null;
    this.educationForm.reset({
      degree: '',
      specialization: '',
      institutionName: '',
      graduationYear: new Date().getFullYear(),
      percentage: null
    });
    this.showModal.set(true);
  }

  openEditModal(edu: EmployeeEducation): void {
    this.editingId = edu.id;
    this.educationForm.patchValue({
      degree: edu.degree,
      specialization: edu.specialization,
      institutionName: edu.institutionName,
      graduationYear: edu.graduationYear,
      percentage: edu.percentage || null
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.educationForm.reset();
  }

  saveEducation(): void {
    if (this.educationForm.invalid) {
      this.educationForm.markAllAsTouched();
      return;
    }

    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.toastr.warning('Please select an employee first.');
      return;
    }

    const formVal = this.educationForm.value;
    this.submitting.set(true);
    const request: AddEmployeeEducation = {
      employeeId: employeeId,
      degree: formVal.degree!,
      specialization: formVal.specialization!,
      institutionName: formVal.institutionName!,
      graduationYear: Number(formVal.graduationYear!),
      percentage: formVal.percentage !== null && formVal.percentage !== undefined ? Number(formVal.percentage) : undefined
    };

    if (this.editingId) {
      this.educationService.updateEducation(this.editingId, request).subscribe({
        next: () => {
          this.toastr.success('Education record updated.');
          this.closeModal();
          this.loadEducations();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.educationService.createEducation(request).subscribe({
        next: () => {
          this.toastr.success('Education record added.');
          this.closeModal();
          this.loadEducations();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteEducation(id: string): void {
    if (confirm('Are you sure you want to delete this education record?')) {
      this.educationService.deleteEducation(id).subscribe({
        next: () => {
          this.toastr.success('Education record deleted.');
          this.loadEducations();
        },
        error: (err) => {
          console.error(err);
        }
      });
    }
  }

  exportEducations(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) return;

    this.educationService.exportEducations(employeeId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `educations_${employeeId.slice(0, 8)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Educations list exported successfully.');
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile.set(input.files[0]);
    }
  }

  importEducations(): void {
    if (!this.selectedFile()) {
      this.toastr.warning('Select an Excel file first.');
      return;
    }
    this.submitting.set(true);

    this.educationService.importEducations(this.selectedFile()!).subscribe({
      next: () => {
        this.toastr.success('Import completed.');
        this.selectedFile.set(null);
        this.loadEducations();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }
}
