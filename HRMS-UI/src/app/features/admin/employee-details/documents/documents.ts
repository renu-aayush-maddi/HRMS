import { Component, OnInit, signal, effect, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { EmployeeDocumentService } from '../../../../core/services/employee-document.service';
import { EmployeeDetailsStore } from '../../../../stores/employee/employee-details.store';
import { EmployeeDocument } from '../../../../core/models/employee-subprofile.model';
import { ToastrService } from 'ngx-toastr';

import {
  LucideDownload,
  LucidePlus,
  LucideCheck,
  LucideTrash2,
  LucideChevronLeft,
  LucideChevronRight,
  LucideX,
  LucideLoader
} from '@lucide/angular';

@Component({
  selector: 'app-documents',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucideDownload,
    LucidePlus,
    LucideCheck,
    LucideTrash2,
    LucideChevronLeft,
    LucideChevronRight,
    LucideX,
    LucideLoader
  ],
  templateUrl: './documents.html',
  styleUrl: './documents.css',
})
export class Documents implements OnInit {
  readonly documents = signal<EmployeeDocument[]>([]);
  readonly loading = signal(false);
  readonly submitting = signal(false);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(10);
  readonly totalPages = signal(0);
  readonly totalRecords = signal(0);

  readonly showUploadModal = signal(false);
  uploadForm;
  
  private toastr = inject(ToastrService);
  readonly selectedFile = signal<File | null>(null);

  documentTypes = ['Identity Proof', 'Address Proof', 'Educational Degree', 'Experience Letter', 'Resume', 'Contract', 'Other'];

  constructor(
    private documentService: EmployeeDocumentService,
    public employeeStore: EmployeeDetailsStore,
    private fb: FormBuilder
  ) {
    this.uploadForm = this.fb.group({
      documentType: ['Identity Proof', Validators.required]
    });

    // Auto-reload on employee changes
    effect(() => {
      const employeeId = this.employeeStore.selectedEmployeeId();
      this.pageNumber.set(1);
      this.loadDocuments();
    });
  }

  ngOnInit(): void {
    // Rely on effect for load
  }

  loadDocuments(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) {
      this.documents.set([]);
      this.totalPages.set(0);
      this.totalRecords.set(0);
      return;
    }

    this.loading.set(true);
    this.documentService.getDocuments(employeeId, this.pageNumber(), this.pageSize()).subscribe({
      next: (response) => {
        this.documents.set(response.data);
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
    this.loadDocuments();
  }

  nextPage(): void {
    if (this.pageNumber() >= this.totalPages()) return;
    this.pageNumber.update(p => p + 1);
    this.loadDocuments();
  }

  openUploadModal(): void {
    this.uploadForm.reset({
      documentType: 'Identity Proof'
    });
    this.selectedFile.set(null);
    this.showUploadModal.set(true);
  }

  closeUploadModal(): void {
    this.showUploadModal.set(false);
    this.selectedFile.set(null);
    this.uploadForm.reset();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile.set(input.files[0]);
    }
  }

  submitUpload(): void {
    if (this.uploadForm.invalid) return;
    if (!this.selectedFile()) {
      this.toastr.warning('Please select a file to upload.');
      return;
    }

    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) return;

    this.submitting.set(true);
    const documentType = this.uploadForm.value.documentType!;
    
    this.documentService.uploadDocument(employeeId, documentType, this.selectedFile()!).subscribe({
      next: () => {
        this.toastr.success('Document uploaded successfully.');
        this.closeUploadModal();
        this.loadDocuments();
        this.submitting.set(false);
      },
      error: (err) => {
        console.error(err);
        this.submitting.set(false);
      }
    });
  }

  verifyDocument(id: string): void {
    if (confirm('Verify this document?')) {
      this.submitting.set(true);
      this.documentService.verifyDocument(id).subscribe({
        next: () => {
          this.toastr.success('Document verified.');
          this.loadDocuments();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  downloadDocument(doc: EmployeeDocument): void {
    this.documentService.downloadDocument(doc.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = doc.documentName;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Document download started.');
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  deleteDocument(id: string): void {
    if (confirm('Are you sure you want to delete this document?')) {
      this.submitting.set(true);
      this.documentService.deleteDocument(id).subscribe({
        next: () => {
          this.toastr.success('Document deleted.');
          this.loadDocuments();
          this.submitting.set(false);
        },
        error: (err) => {
          console.error(err);
          this.submitting.set(false);
        }
      });
    }
  }

  exportDocuments(): void {
    const employeeId = this.employeeStore.selectedEmployeeId();
    if (!employeeId) return;

    this.documentService.exportDocuments(employeeId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `documents_${employeeId.slice(0, 8)}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success('Documents list exported successfully.');
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}
