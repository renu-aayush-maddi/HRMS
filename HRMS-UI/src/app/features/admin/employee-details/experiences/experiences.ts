import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { EmployeeExperienceService } from '../../../../core/services/employee-experience.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { EmployeeExperience, AddEmployeeExperience } from '../../../../core/models/employee-subprofile.model';
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
  selector: 'app-experiences',
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
  templateUrl: './experiences.html',
  styleUrl: './experiences.css',
})
export class Experiences implements OnInit {
  readonly experiences = signal<EmployeeExperience[]>([]);
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
  experienceForm;
  readonly selectedFile = signal<File | null>(null);

  constructor(
    private experienceService: EmployeeExperienceService,
    public employeeStore: EmployeeDetailsStore,
    private fb: FormBuilder
  ) {
    this.experienceForm = this.fb.group({
      companyName: ['', [Validators.required, Validators.maxLength(150)]],
      designation: ['', [Validators.required, Validators.maxLength(100)]],
      startDate: ['', [Validators.required]],
      endDate: [''],
      responsibilities: ['', [Validators.maxLength(1000)]]
    });

    // Auto-reload on employee changes
    effect(() => {
      const employeeId = this.employeeStore.selectedEmployeeId();
      this.pageNumber.set(1);
      this.loadExperiences();
    });
  }

  ngOnInit(): void {
    // Rely on effect for load
  }

  loadExperiences(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.experiences.set([]);
      this.totalPages.set(0);
      this.totalRecords.set(0);
      return;
    }

    this.loading.set(true);
    this.experienceService.getExperiences(employeeId, this.pageNumber(), this.pageSize()).subscribe({
      next: (response) => {
        this.experiences.set(response.data);
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
    this.loadExperiences();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.pageNumber.update(p => p + 1);
    this.loadExperiences();
  }

  openAddModal(): void {
    this.editingId = null;
    this.experienceForm.reset({
      companyName: '',
      designation: '',
      startDate: '',
      endDate: '',
      responsibilities: ''
    });
    this.showModal.set(true);
  }

  openEditModal(exp: EmployeeExperience): void {
    this.editingId = exp.id;
    this.experienceForm.patchValue({
      companyName: exp.companyName,
      designation: exp.designation,
      startDate: exp.startDate || '',
      endDate: exp.endDate || '',
      responsibilities: exp.responsibilities || ''
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId = null;
    this.experienceForm.reset();
  }

  saveExperience(): void {
    if (this.experienceForm.invalid) {
      this.experienceForm.markAllAsTouched();
      return;
    }

    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.toastr.warning('Please select an employee first.');
      return;
    }

    const formVal = this.experienceForm.value;
    this.submitting.set(true);
    const request: AddEmployeeExperience = {
      employeeId: employeeId,
      companyName: formVal.companyName!,
      designation: formVal.designation!,
      startDate: formVal.startDate!,
      endDate: formVal.endDate || undefined,
      responsibilities: formVal.responsibilities || undefined
    };

    if (this.editingId) {
      this.experienceService.updateExperience(this.editingId, request).subscribe({
        next: () => {
          this.toastr.success('Experience record updated.');
          this.closeModal();
          this.loadExperiences();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    } else {
      this.experienceService.createExperience(request).subscribe({
        next: () => {
          this.toastr.success('Experience record added.');
          this.closeModal();
          this.loadExperiences();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  deleteExperience(id: string): void {
    if (confirm('Are you sure you want to delete this experience record?')) {
      this.experienceService.deleteExperience(id).subscribe({
        next: () => {
          this.toastr.success('Experience record deleted.');
          this.loadExperiences();
        },
        error: (err) => {
          console.error(err);
        }
      });
    }
  }

  exportExperiences(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) return;

    this.experienceService.exportExperiences(employeeId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `experiences_${employeeId.slice(0, 8)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Experiences list exported successfully.');
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

  importExperiences(): void {
    if (!this.selectedFile()) {
      this.toastr.warning('Select an Excel file first.');
      return;
    }
    this.submitting.set(true);

    this.experienceService.importExperiences(this.selectedFile()!).subscribe({
      next: () => {
        this.toastr.success('Import completed.');
        this.selectedFile.set(null);
        this.loadExperiences();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }
}
